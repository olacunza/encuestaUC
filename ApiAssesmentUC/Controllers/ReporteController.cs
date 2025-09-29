using AssesmentUC.Service.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentUC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("GenerarPdfEncuesta/{id}")]
        public async Task<IActionResult> GenerarPdfEncuesta(int id)
        {
            try
            {
                var pdfBytes = await _reporteService.GenerarPdfEncuesta(id);
                return File(pdfBytes, "application/pdf", $"encuesta_{id}_{DateTime.Now}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar PDF: {ex.Message}" });
            }
        }

        [HttpGet("GenerarExcelEncuesta/{id}")]
        public async Task<IActionResult> GenerarExcelEncuesta(int id)
        {
            try
            {
                var excelBytes = await _reporteService.GenerarExcelEncuesta(id);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"encuesta_{id}_{DateTime.Now}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar Excel: {ex.Message}" });
            }
        }
    }
}
