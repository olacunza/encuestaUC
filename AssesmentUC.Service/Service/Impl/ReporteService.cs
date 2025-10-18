using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Service.DTO;
using AssesmentUC.Service.Service.Interface;
using DinkToPdf.Contracts;
using static AssesmentUC.Model.Entity.ReporteEncuestas;
using static AssesmentUC.Service.DTO.Reporte.ReporteEncuestaDTO;

namespace AssesmentUC.Service.Service.Impl
{
    public class ReporteService: IReporteService
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly IConverter _pdfConverter;
        public ReporteService(IReporteRepository reporteRepository, IConverter pdfConverter)
        {
            _reporteRepository = reporteRepository;
            _pdfConverter = pdfConverter;
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

        //public async Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId)
        //{
        //    var encuesta = await _reporteRepository.ListarEncuestaRepository(/*encuestaId*/);

        //    if (encuesta == null)
        //        throw new Exception("Encuesta no encontrada");

        //    var dto = new EncuestaExportarPdfDTO
        //    {
        //        NombreEncuesta = encuesta.NombreEncuesta,
        //        DescripcionEncuesta = encuesta.DescripcionEncuesta,
        //        Periodo = encuesta.Periodo,
        //        Seccion = encuesta.Seccion,
        //        FechaInicio = encuesta.FechaInicio,
        //        FechaFin = encuesta.FechaFin,
        //        Bloques = encuesta.Bloques.Select(b => new BloqueExportPdfDTO
        //        {
        //            TituloBloque = b.TituloBloque,
        //            Orden = b.OrdenBloque,
        //            Preguntas = b.Preguntas.Select(p => new PreguntaExportPdfDTO
        //            {
        //                TextoPregunta = p.TextoPregunta,
        //                TipoPregunta = p.TipoPregunta,
        //                Orden = p.OrdenPregunta
        //            }).ToList()
        //        }).ToList()
        //    };

        //    return dto;
        //}

        public async Task<byte[]> GenerarPdfEncuestaAlumno(int encuestaId)
        {
            return null;
        }

        public async Task<byte[]> GenerarPdfEncuestaDocente(int encuestaId)
        {
            var entity = await _reporteRepository.ExportarValoresEncuestaDocente(encuestaId);

            var viewModel = new ReporteEncuestaDocenteViewModel
            {
                NombreEncuesta = entity.NombreEncuesta,
                Docente = entity.Docente,
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
                throw new DirectoryNotFoundException($"❌ No se encontró la carpeta Templates en: {templatePath}");

            var fullPath = Path.GetFullPath(templatePath);

            var engine = new RazorLight.RazorLightEngineBuilder()
                .UseFileSystemProject(fullPath)
                .UseMemoryCachingProvider()
                .Build();

            var html = await engine.CompileRenderAsync("ReporteDocentePlantilla.cshtml", viewModel);

            var renderer = new ChromePdfRenderer();
            renderer.RenderingOptions.MarginTop = 20;
            renderer.RenderingOptions.MarginBottom = 20;
            renderer.RenderingOptions.MarginLeft = 15;
            renderer.RenderingOptions.MarginRight = 15;

            using var pdf = renderer.RenderHtmlAsPdf(html);
            return pdf.BinaryData;

        }

        public async Task<byte[]> GenerarPdfEncuestaAsesor(int encuestaId)
        {
            return null;
        }

        //public async Task<byte[]> GenerarPdfEncuesta(int encuestaId)
        //{
        //    var dto = await ObtenerEncuestaParaExportar(encuestaId);

        //    using var stream = new MemoryStream();
        //    var document = new PdfDocument();
        //    var page = document.AddPage();
        //    var gfx = XGraphics.FromPdfPage(page);
        //    var font = new XFont("Arial", 12, XFontStyle.Regular);

        //    int y = 40;
        //    gfx.DrawString($"Encuesta: {dto.NombreEncuesta}", font, XBrushes.Black, new XPoint(40, y));
        //    y += 25;
        //    gfx.DrawString($"Periodo: {dto.Periodo}  Sección: {dto.Seccion}", font, XBrushes.Black, new XPoint(40, y));
        //    y += 25;

        //    foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
        //    {
        //        gfx.DrawString($"Bloque: {bloque.TituloBloque}", font, XBrushes.DarkBlue, new XPoint(40, y));
        //        y += 20;

        //        foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
        //        {
        //            gfx.DrawString($"• {pregunta.TextoPregunta} ({pregunta.TipoPregunta})", font, XBrushes.Black, new XPoint(60, y));
        //            y += 18;
        //        }

        //        y += 10;
        //    }

        //    document.Save(stream, false);
        //    return stream.ToArray();
        //}

        //public async Task<byte[]> GenerarExcelEncuesta(int encuestaId)
        //{
        //    var dto = await ObtenerEncuestaParaExportar(encuestaId);

        //    using var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("Encuesta");

        //    int row = 1;

        //    worksheet.Cell(row++, 1).Value = $"Nombre de Encuesta: {dto.NombreEncuesta}";
        //    worksheet.Cell(row++, 1).Value = $"Periodo: {dto.Periodo ?? "-"}";
        //    worksheet.Cell(row++, 1).Value = $"Sección: {dto.Seccion ?? "-"}";
        //    row++;

        //    foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
        //    {
        //        worksheet.Cell(row++, 1).Value = $"Bloque: {bloque.TituloBloque}";

        //        worksheet.Cell(row, 1).Value = "N°";
        //        worksheet.Cell(row, 2).Value = "Texto de la Pregunta";
        //        worksheet.Cell(row, 3).Value = "Tipo";
        //        row++;

        //        foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
        //        {
        //            worksheet.Cell(row, 1).Value = pregunta.Orden;
        //            worksheet.Cell(row, 2).Value = pregunta.TextoPregunta;
        //            worksheet.Cell(row, 3).Value = pregunta.TipoPregunta;
        //            row++;
        //        }

        //        row++;
        //    }

        //    using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    return stream.ToArray();
        //}
    }
}
