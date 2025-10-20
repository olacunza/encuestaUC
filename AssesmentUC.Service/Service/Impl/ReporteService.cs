using AssesmentUC.Infrastructure.Repository.Impl;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.ViewModel;
using AssesmentUC.Service.Service.Interface;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using static AssesmentUC.Model.Entity.ReporteEncuestas;
using static AssesmentUC.Service.DTO.Reporte.ReporteEncuestaDTO;

namespace AssesmentUC.Service.Service.Impl
{
    public class ReporteService: IReporteService
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly IRespuestaRepository _respuestaRepository;
        public ReporteService(IReporteRepository reporteRepository, IRespuestaRepository respuestaRepository)
        {
            _reporteRepository = reporteRepository;
            _respuestaRepository = respuestaRepository;
        }

        public async Task<ReporteEncuestaDto> ExportarValoresEncuestaAlumno(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaAlumno(encuestaId);
            return MapToDto(entity);
        }

        public async Task<ReporteEncuestaDto> ExportarValoresEncuestaDocente(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaDocente(encuestaId);
            return MapToDto(entity);
        }

        public async Task<ReporteEncuestaDto> ExportarValoresEncuestaAsesor(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaAsesor(encuestaId);
            return MapToDto(entity);
        }

        private ReporteEncuestaDto MapToDto(ReporteEncuesta entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "El reporte recibido desde la base de datos es nulo.");

            return new ReporteEncuestaDto
            {
                EncuestaId = entity.EncuestaId,
                TipoEncuestado = entity.TipoEncuestado ?? string.Empty,

                Resumen = entity.Resumen == null ? null : new ReporteResumenDto
                {
                    PromedioTotalScore = Math.Round(entity.Resumen.PromedioTotalScore ?? 0, 2),
                    CantidadMatriculados = entity.Resumen.CantidadMatriculados,
                    CantidadEncuestados = entity.Resumen.CantidadEncuestados,
                    PorcentajeEncuestados = Math.Round(entity.Resumen.PorcentajeEncuestados ?? 0, 2)
                },

                Bloques = entity.Bloques?.Select(b => new ReporteBloqueDto
                {
                    BloqueId = b.BloqueId,
                    TituloBloque = b.TituloBloque ?? "(Sin título)",
                    PromedioBloque = Math.Round(b.PromedioBloque ?? 0, 2),

                    Preguntas = b.Preguntas?.Select(p => new ReportePreguntaDto
                    {
                        EncuestaPreguntaId = p.EncuestaPreguntaId,
                        TextoPregunta = p.TextoPregunta ?? string.Empty,
                        Promedio = Math.Round(p.Promedio, 2),
                        Pct1 = p.PCT1 ?? 0,
                        Pct2 = p.PCT2 ?? 0,
                        Pct3 = p.PCT3 ?? 0,
                        Pct4 = p.PCT4 ?? 0,
                        Pct5 = p.PCT5 ?? 0
                    }).ToList() ?? new List<ReportePreguntaDto>()
                }).ToList() ?? new List<ReporteBloqueDto>()
            };
        }

        public async Task<byte[]> GenerarPdfEncuestaAlumno(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaAlumno(encuestaId);

            string NombreDocente = await _respuestaRepository.BuscarNombreDocente(entity.Docente);

            var viewModel = new ReporteEncuestaAlumnoViewModel
            {
                NombreEncuesta = entity.NombreEncuesta,
                Programa = entity.Programa,
                Seccion = entity.Seccion,
                Asignatura = entity.Asignatura,
                Docente = NombreDocente,

                Resumen = new ReporteResumenViewModel
                {
                    CantidadMatriculados = entity.Resumen?.CantidadMatriculados ?? 0,
                    CantidadEncuestados = entity.Resumen?.CantidadEncuestados ?? 0,
                    PorcentajeEncuestados = entity.Resumen?.PorcentajeEncuestados ?? 0,
                    PromedioTotalScore = entity.Resumen?.PromedioTotalScore ?? 0
                },
                Bloques = entity.Bloques.Select(b => new ReporteBloqueViewModel
                {
                    BloqueId = b.BloqueId,
                    TituloBloque = b.TituloBloque,
                    PromedioBloque = b.PromedioBloque,
                    Preguntas = b.Preguntas.Select(p => new ReportePreguntaViewModel
                    {
                        TextoPregunta = p.TextoPregunta,
                        Promedio = p.Promedio,
                        Pct1 = p.PCT1,
                        Pct2 = p.PCT2,
                        Pct3 = p.PCT3,
                        Pct4 = p.PCT4,
                        Pct5 = p.PCT5
                    }).ToList()
                }).ToList()
            };

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");

            if (!Directory.Exists(templatePath))
            {
                templatePath = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AssesmentUC.Service", "Templates")
                );
            }

            if (!Directory.Exists(templatePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta Templates en: {templatePath}");

            var fullPath = Path.GetFullPath(templatePath);

            var engine = new RazorLight.RazorLightEngineBuilder()
                .UseFileSystemProject(fullPath)
                .UseMemoryCachingProvider()
                .Build();

            var html = await engine.CompileRenderAsync("ReporteAlumnoPlantilla.cshtml", viewModel);

            var chromiumPath = Path.Combine(AppContext.BaseDirectory, "chromium");
            var fetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = chromiumPath
            });

            var revisionInfo = await fetcher.DownloadAsync();

            var executablePath = revisionInfo.GetExecutablePath();

            if (!File.Exists(executablePath))
            {
                throw new FileNotFoundException($"No se encontró el ejecutable de Chromium en: {executablePath}");
            }

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });


            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new NavigationOptions { Timeout = 0 });

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions
                {
                    Top = "20px",
                    Bottom = "20px",
                    Left = "15px",
                    Right = "15px"
                },
                PrintBackground = true
            });

            return pdfBytes;
        }

        public async Task<byte[]> GenerarPdfEncuestaDocente(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaDocente(encuestaId);

            string NombreDocente = await _respuestaRepository.BuscarNombreDocente(entity.Docente);

            var viewModel = new ReporteEncuestaDocenteViewModel
            {
                NombreEncuesta = entity.NombreEncuesta,
                Docente = NombreDocente,
                Periodo = entity.Periodo,
                Seccion = entity.Seccion,
                FechaInicio = entity.FechaInicio,
                FechaFin = entity.FechaFin,
                Bloques = entity.Bloques
            };

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");

            if (!Directory.Exists(templatePath))
            {
                templatePath = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AssesmentUC.Service", "Templates")
                );
            }

            if (!Directory.Exists(templatePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta Templates en: {templatePath}");

            var fullPath = Path.GetFullPath(templatePath);

            var engine = new RazorLight.RazorLightEngineBuilder()
                .UseFileSystemProject(fullPath)
                .UseMemoryCachingProvider()
                .Build();

            var html = await engine.CompileRenderAsync("ReporteDocentePlantilla.cshtml", viewModel);

            // Descarga y localiza Chromium
            var chromiumPath = Path.Combine(AppContext.BaseDirectory, "chromium");
            var fetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = chromiumPath
            });

            // Descarga si no existe (solo la primera vez)
            var revisionInfo = await fetcher.DownloadAsync();

            // Verifica la ruta real del ejecutable descargado
            var executablePath = revisionInfo.GetExecutablePath();

            if (!File.Exists(executablePath))
            {
                throw new FileNotFoundException($"No se encontró el ejecutable de Chromium en: {executablePath}");
            }

            // Lanza Puppeteer apuntando al ejecutable real
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath, // ruta explícita
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });


            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions
                {
                    Top = "20px",
                    Bottom = "20px",
                    Left = "15px",
                    Right = "15px"
                },
                PrintBackground = true
            });

            return pdfBytes;

        }

        public async Task<byte[]> GenerarPdfEncuestaAsesor(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaAsesor(encuestaId);

            string NombreDocente = await _respuestaRepository.BuscarNombreDocente(entity.Docente);

            var viewModel = new ReporteEncuestaAsesorViewModel
            {
                Programa = entity.Programa,
                Seccion = entity.Seccion,
                Curso = entity.Asignatura,
                Docente = NombreDocente,

                Preguntas = entity.Bloques
                    .SelectMany(b => b.Preguntas.Select((p, index) => new PreguntaViewModel
                    {
                        Nro = index + 1,
                        TextoPregunta = p.TextoPregunta,
                        Promedio = p.Promedio
                    }))
                    .ToList(),

                PromedioArea = entity.Bloques.Average(b => b.PromedioBloque ?? 0),

                PromedioEstudiantes = entity.Resumen?.PromedioTotalScore,
                ModeloAcademico = entity.Resumen?.PromedioTotalScore,
                Comentarios = ""    //VALIDAR SI IRÁ ESTE CAMPO O NO EN EL PDF
            };

            viewModel.EvaluacionFinal = ((viewModel.PromedioEstudiantes ?? 0) * 0.75m) 
                                        + ((viewModel.PromedioArea ?? 0) * 0.15m) 
                                        + ((viewModel.ModeloAcademico ?? 0) * 0.10m);

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");

            if (!Directory.Exists(templatePath))
            {
                templatePath = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AssesmentUC.Service", "Templates")
                );
            }

            if (!Directory.Exists(templatePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta Templates en: {templatePath}");

            var fullPath = Path.GetFullPath(templatePath);

            var engine = new RazorLight.RazorLightEngineBuilder()
                .UseFileSystemProject(fullPath)
                .UseMemoryCachingProvider()
                .Build();

            var html = await engine.CompileRenderAsync("ReporteAsesorPlantilla.cshtml", viewModel);

            var chromiumPath = Path.Combine(AppContext.BaseDirectory, "chromium");
            var fetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = chromiumPath
            });

            var revisionInfo = await fetcher.DownloadAsync();

            var executablePath = revisionInfo.GetExecutablePath();

            if (!File.Exists(executablePath))
            {
                throw new FileNotFoundException($"No se encontró el ejecutable de Chromium en: {executablePath}");
            }

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });


            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions
                {
                    Top = "20px",
                    Bottom = "20px",
                    Left = "15px",
                    Right = "15px"
                },
                PrintBackground = true
            });

            return pdfBytes;
        }

        public async Task<byte[]> GenerarExcelEncuesta(int id)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaExcel(id);

            string NombreDocente = await _respuestaRepository.BuscarNombreDocente(entity.Hoja1.Docente);

            using (var workbook = new XLWorkbook())
            {
                // ===========================
                //  HOJA 1 - Resumen Encuesta (Formato Final Mejorado)
                // ===========================
                var hojaAlumno = workbook.Worksheets.Add("Resumen Encuesta");

                // === Estilo general ===
                hojaAlumno.Style.Font.FontName = "Calibri";
                hojaAlumno.Style.Font.FontSize = 11;
                hojaAlumno.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // === Cabecera principal ===

                hojaAlumno.Range("A2:B2").Unmerge();
                // Hacer que toda la columna B permita texto multilínea
                hojaAlumno.Column(2).Style.Alignment.SetWrapText(true);
                hojaAlumno.Rows().AdjustToContents();

                // Asegurar anchos de columnas antes del título
                hojaAlumno.Column(1).Width = 45; // Columna A
                hojaAlumno.Column(2).Width = 45; // Columna B

                // Combinar celdas y centrar texto
                var tituloRango = hojaAlumno.Range("A2:B2");
                tituloRango.Merge();
                tituloRango.Value = "RESULTADO DE ENCUESTA";
                tituloRango.Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Ajustar altura de fila
                hojaAlumno.Row(2).Height = 25;


                // Ajuste de altura y ancho
                hojaAlumno.Row(2).Height = 25;
                hojaAlumno.Column(1).Width = 40;
                hojaAlumno.Column(2).Width = 40;


                // === Datos generales ===
                hojaAlumno.Cell("A4").Value = "PROGRAMA:";
                hojaAlumno.Cell("A5").Value = "ASIGNATURA:";
                hojaAlumno.Cell("A6").Value = "DOCENTE:";
                hojaAlumno.Cell("A7").Value = "FECHAS:";

                hojaAlumno.Cell("B4").Value = entity.Hoja1.Programa;
                hojaAlumno.Cell("B5").Value = entity.Hoja1.Asignatura;
                hojaAlumno.Cell("B6").Value = NombreDocente;
                hojaAlumno.Cell("B7").Value = entity.Hoja1.Fechas;

                hojaAlumno.Range("A4:A7").Style.Font.SetBold();

                hojaAlumno.Range("B4:B7").Style
                    .Font.SetBold() // texto en negrita
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetWrapText(true)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Altura para mostrar texto multilínea
                for (int i = 4; i <= 7; i++)
                {
                    hojaAlumno.Row(i).Height = 30;
                }

                // === Encabezado de tabla ===
                int startRow = 9;
                hojaAlumno.Cell(startRow, 1).Value = "ENCUESTA";
                hojaAlumno.Cell(startRow, 2).Value = NombreDocente;

                var headerRange = hojaAlumno.Range(startRow, 1, startRow, 2);
                headerRange.Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // === Contenido de bloques ===
                int row = startRow + 1;
                foreach (var bloque in entity.Hoja1.Bloques)
                {
                    // Fila de bloque (con promedio del bloque)
                    hojaAlumno.Cell(row, 1).Value = bloque.TituloBloque;
                    hojaAlumno.Cell(row, 2).Value = bloque.PromedioBloque;

                    hojaAlumno.Range(row, 1, row, 2).Style
                        .Fill.SetBackgroundColor(XLColor.Black)
                        .Font.SetBold()
                        .Font.SetFontColor(XLColor.White)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                    row++;

                    // Preguntas del bloque
                    foreach (var p in bloque.Preguntas)
                    {
                        hojaAlumno.Cell(row, 1).Value = p.TextoPregunta;
                        hojaAlumno.Cell(row, 2).Value = p.PromedioPregunta;

                        hojaAlumno.Cell(row, 1).Style.Alignment.SetWrapText(true); // ajuste de línea pregunta
                        hojaAlumno.Cell(row, 2).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetWrapText(true); // ajuste de línea promedio

                        hojaAlumno.Range(row, 1, row, 2).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Font.SetFontSize(11)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        hojaAlumno.Row(row).Height = 30; // altura uniforme
                        row++;
                    }
                }

                // Ajustar anchos
                hojaAlumno.Column(1).Width = 75; // Pregunta
                hojaAlumno.Column(2).Width = 40; // Promedio

                // Evitar encogimiento de texto
                hojaAlumno.Style.Alignment.SetShrinkToFit(false);


                // ============================
                // HOJA 2: RESPUESTAS INDIVIDUALES
                // ============================
                var hoja2 = workbook.Worksheets.Add("Respuestas");

                // === Título ===
                var tituloRango2 = hoja2.Range("A1:H1");
                tituloRango2.Merge();
                tituloRango2.Value = "RESPUESTAS INDIVIDUALES POR ENCUESTA";
                tituloRango2.Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // === Fila de cabeceras dinámicas ===
                hoja2.Cell(3, 1).Value = "EncuestaId";

                // Diccionario de preguntas (id -> texto)
                var mapaPreguntas = entity.Hoja1.Bloques
                    .SelectMany(b => b.Preguntas)
                    .ToDictionary(p => p.PreguntaId, p => p.TextoPregunta);

                // IDs de pregunta
                var preguntaIds = entity.Hoja2
                    .SelectMany(r => r.Respuestas.Keys)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                int col = 2;
                foreach (var pid in preguntaIds)
                {
                    var texto = mapaPreguntas.ContainsKey(pid)
                        ? mapaPreguntas[pid]
                        : $"Pregunta {pid}";

                    var celda = hoja2.Cell(3, col);
                    celda.Value = texto;

                    // ✅ Rotación vertical (mirando hacia arriba)
                    celda.Style.Alignment.SetTextRotation(90);
                    celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    celda.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);

                    // Estilos adicionales
                    celda.Style.Font.SetBold();
                    celda.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    celda.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                    col++;
                }

                // Última cabecera (Comentario sin rotación)
                var celdaComentario = hoja2.Cell(3, col);
                celdaComentario.Value = "Comentario";
                celdaComentario.Style.Font.SetBold();
                celdaComentario.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                celdaComentario.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                celdaComentario.Style.Alignment.SetWrapText(true);
                celdaComentario.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                celdaComentario.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Ajuste de anchos y altura
                hoja2.Column(1).Width = 15;
                for (int i = 2; i < col; i++)
                    hoja2.Column(i).Width = 6;
                hoja2.Column(col).Width = 25;
                hoja2.Row(3).Height = 90;

                // ============================
                // CARGA DE RESPUESTAS INDIVIDUALES
                // ============================

                int currentRow = 4; // Comienza debajo de las cabeceras

                foreach (var respuesta in entity.Hoja2)
                {
                    // Columna 1: ID de la encuesta
                    hoja2.Cell(currentRow, 1).Value = respuesta.EncuestaId;

                    int colIndex = 2;
                    foreach (var pid in preguntaIds)
                    {
                        if (respuesta.Respuestas.TryGetValue(pid, out var valor))
                        {
                            hoja2.Cell(currentRow, colIndex).Value = valor;
                        }
                        else
                        {
                            hoja2.Cell(currentRow, colIndex).Value = "";
                        }

                        hoja2.Cell(currentRow, colIndex).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        colIndex++;
                    }

                    // Última columna: Comentario
                    hoja2.Cell(currentRow, colIndex).Value = respuesta.Comentario ?? "";
                    hoja2.Cell(currentRow, colIndex).Style
                        .Alignment.SetWrapText(true)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                    hoja2.Row(currentRow).Height = 25;
                    currentRow++;
                }

                // ============================
                // HOJA 3: RESUMEN FINAL
                // ============================

                var hoja3 = workbook.Worksheets.Add("Resumen Final");

                // === Estilo base general ===
                hoja3.Style.Font.FontName = "Calibri";
                hoja3.Style.Font.FontSize = 11;
                hoja3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                hoja3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                // === Insertar logos ===
                string logo1Path = Path.Combine(AppContext.BaseDirectory, "Templates", "Logos", "logo-uc1.png");
                string logo2Path = Path.Combine(AppContext.BaseDirectory, "Templates", "Logos", "logo-uc-continua.png");

                if (File.Exists(logo1Path))
                {
                    hoja3.AddPicture(logo1Path)
                        .MoveTo(hoja3.Cell("A1"))
                        .WithSize(120, 60);
                }
                if (File.Exists(logo2Path))
                {
                    hoja3.AddPicture(logo2Path)
                        .MoveTo(hoja3.Cell("E1"))
                        .WithSize(120, 60);
                }

                // === Título principal ===
                var tituloRangoH3 = hoja3.Range("A4:F4");
                tituloRangoH3.Merge();
                tituloRangoH3.Value = "ENCUESTA POR ASIGNATURA";
                tituloRangoH3.Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Font.SetFontColor(XLColor.FromHtml("#1F497D"))
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                hoja3.Row(4).Height = 25;

                // === Datos generales ===
                hoja3.Cell("A6").Value = "PROGRAMA:";
                hoja3.Cell("C6").Value = entity.Hoja1.Programa;

                hoja3.Cell("A7").Value = "ASIGNATURA:";
                hoja3.Cell("C7").Value = entity.Hoja1.Asignatura;

                hoja3.Cell("A8").Value = "DOCENTE:";
                hoja3.Cell("C8").Value = entity.Hoja1.Docente;

                hoja3.Cell("A9").Value = "FECHA:";
                hoja3.Cell("C9").Value = entity.Hoja1.Fechas;

                hoja3.Range("A6:A9").Style.Font.SetBold();
                hoja3.Range("C6:C9").Style.Alignment.SetWrapText(true);
                hoja3.Rows(6, 9).Height = 30;

                // === Configurar anchos de columnas ===
                hoja3.Column(1).Width = 4;
                hoja3.Column(2).Width = 48;  // Columna B para texto largo (preguntas)
                hoja3.Column(3).Width = 14;
                hoja3.Column(4).Width = 20;
                hoja3.Column(5).Width = 12;
                hoja3.Column(6).Width = 8;

                // === Contenido de bloques ===
                int currentRowH3 = 11;

                var bloquesUnicos = entity.Hoja1.Bloques
                    .GroupBy(b => b.TituloBloque.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

                foreach (var bloque in bloquesUnicos)
                {
                    // === Título del bloque (columna B) ===
                    hoja3.Cell(currentRowH3, 2).Value = bloque.TituloBloque?.ToUpper();
                    hoja3.Cell(currentRowH3, 2).Style
                        .Font.SetBold()
                        .Font.SetFontSize(12)
                        .Font.SetFontColor(XLColor.FromHtml("#1F497D"));
                    hoja3.Row(currentRowH3).Height = 24;
                    currentRowH3 += 2;

                    // === Preguntas ===
                    foreach (var pregunta in bloque.Preguntas)
                    {
                        hoja3.Cell(currentRowH3, 2).Value = pregunta.TextoPregunta;
                        hoja3.Cell(currentRowH3, 5).Value = pregunta.PromedioPregunta;
                        hoja3.Cell(currentRowH3, 5).Style.NumberFormat.SetFormat("0.00");

                        hoja3.Range(currentRowH3, 2, currentRowH3, 5).Style
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetFontSize(11);
                        hoja3.Row(currentRowH3).Height = 22;
                        currentRowH3++;
                    }

                    // === Promedio del bloque ===
                    hoja3.Cell(currentRowH3, 4).Value = "Promedio";
                    hoja3.Cell(currentRowH3, 5).Value = bloque.PromedioBloque;
                    hoja3.Cell(currentRowH3, 5).Style.NumberFormat.SetFormat("0.00");

                    hoja3.Range(currentRowH3, 4, currentRowH3, 5).Style
                        .Font.SetBold()
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"))
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    currentRowH3 += 2;
                }

                // === Promedio final ===
                hoja3.Cell(currentRowH3, 3).Value = "PROMEDIO FINAL";
                hoja3.Cell(currentRowH3, 5).Value = entity.Hoja1.PromedioTotal;
                hoja3.Cell(currentRowH3, 5).Style.NumberFormat.SetFormat("0.00");

                hoja3.Range(currentRowH3, 3, currentRowH3, 5).Style
                    .Font.SetBold()
                    .Font.SetFontSize(12)
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                currentRowH3 += 2;

                // === Cantidad de encuestados ===
                hoja3.Cell(currentRowH3, 3).Value = "CANTIDAD DE ENCUESTADOS";
                hoja3.Cell(currentRowH3, 5).Value = entity.Hoja3.CantidadEncuestados;
                hoja3.Range(currentRowH3, 3, currentRowH3, 5).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                currentRowH3 += 3;

                // === Comentarios ===
                hoja3.Cell(currentRowH3, 2).Value = "Comentarios";
                hoja3.Cell(currentRowH3, 2).Style
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromHtml("#1F497D"))
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                currentRowH3++;

                //foreach (var comentario in entity.Hoja3.Comentarios)
                //{
                //    hoja3.Cell(currentRowH3, 2).Value = comentario;
                //    hoja3.Cell(currentRowH3, 2).Style
                //        .Alignment.SetWrapText(true)
                //        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                //    hoja3.Row(currentRowH3).Height = 30;
                //    currentRowH3++;
                //}

                hoja3.SheetView.FreezeRows(5);

                // ============================
                // GENERAR ARCHIVO
                // ============================
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
