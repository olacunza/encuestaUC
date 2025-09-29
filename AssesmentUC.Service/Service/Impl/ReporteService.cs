using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Service.DTO.Encuesta;
using AssesmentUC.Service.Service.Interface;
using ClosedXML.Excel;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace AssesmentUC.Service.Service.Impl
{
    public class ReporteService: IReporteService
    {
        private readonly IReporteRepository _reporteRepository;
        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }
        public async Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId)
        {
            var encuesta = await _reporteRepository.ListarEncuestaRepository(/*encuestaId*/);

            if (encuesta == null)
                throw new Exception("Encuesta no encontrada");

            var dto = new EncuestaExportarPdfDTO
            {
                NombreEncuesta = encuesta.NombreEncuesta,
                DescripcionEncuesta = encuesta.DescripcionEncuesta,
                Periodo = encuesta.Periodo,
                Seccion = encuesta.Seccion,
                FechaInicio = encuesta.FechaInicio,
                FechaFin = encuesta.FechaFin,
                Bloques = encuesta.Bloques.Select(b => new BloqueExportPdfDTO
                {
                    TituloBloque = b.TituloBloque,
                    Orden = b.OrdenBloque,
                    Preguntas = b.Preguntas.Select(p => new PreguntaExportPdfDTO
                    {
                        TextoPregunta = p.TextoPregunta,
                        TipoPregunta = p.TipoPregunta,
                        Orden = p.OrdenPregunta
                    }).ToList()
                }).ToList()
            };

            return dto;
        }

        public async Task<byte[]> GenerarPdfEncuesta(int encuestaId)
        {
            var dto = await ObtenerEncuestaParaExportar(encuestaId);

            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            int y = 40;
            gfx.DrawString($"Encuesta: {dto.NombreEncuesta}", font, XBrushes.Black, new XPoint(40, y));
            y += 25;
            gfx.DrawString($"Periodo: {dto.Periodo}  Sección: {dto.Seccion}", font, XBrushes.Black, new XPoint(40, y));
            y += 25;

            foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
            {
                gfx.DrawString($"Bloque: {bloque.TituloBloque}", font, XBrushes.DarkBlue, new XPoint(40, y));
                y += 20;

                foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
                {
                    gfx.DrawString($"• {pregunta.TextoPregunta} ({pregunta.TipoPregunta})", font, XBrushes.Black, new XPoint(60, y));
                    y += 18;
                }

                y += 10;
            }

            document.Save(stream, false);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerarExcelEncuesta(int encuestaId)
        {
            var dto = await ObtenerEncuestaParaExportar(encuestaId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Encuesta");

            int row = 1;

            worksheet.Cell(row++, 1).Value = $"Nombre de Encuesta: {dto.NombreEncuesta}";
            worksheet.Cell(row++, 1).Value = $"Periodo: {dto.Periodo ?? "-"}";
            worksheet.Cell(row++, 1).Value = $"Sección: {dto.Seccion ?? "-"}";
            row++;

            foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
            {
                worksheet.Cell(row++, 1).Value = $"Bloque: {bloque.TituloBloque}";

                worksheet.Cell(row, 1).Value = "N°";
                worksheet.Cell(row, 2).Value = "Texto de la Pregunta";
                worksheet.Cell(row, 3).Value = "Tipo";
                row++;

                foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
                {
                    worksheet.Cell(row, 1).Value = pregunta.Orden;
                    worksheet.Cell(row, 2).Value = pregunta.TextoPregunta;
                    worksheet.Cell(row, 3).Value = pregunta.TipoPregunta;
                    row++;
                }

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
