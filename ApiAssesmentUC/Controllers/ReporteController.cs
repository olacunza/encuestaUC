using AssesmentUC.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentUC.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("ExportarValoresEncuestaDocente")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportarValoresEncuestaDocente(int encuestaId)
        {
            var valoresDocente = await _reporteService.ExportarValoresEncuestaDocente(encuestaId);

            if (valoresDocente == null)
                return NotFound("No se encontraron resultados para la encuesta especificada.");

            return Ok(valoresDocente);
        }

        [HttpGet("ExportarValoresEncuestaAsesor")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportarValoresEncuestaAsesor(int encuestaId)
        {
            var valoresDocente = await _reporteService.ExportarValoresEncuestaAsesor(encuestaId);

            if (valoresDocente == null)
                return NotFound("No se encontraron resultados para la encuesta especificada.");

            return Ok(valoresDocente);
        }

        [HttpGet("ExportarValoresEncuestaAlumno")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportarValoresEncuestaAlumno(int encuestaId)
        {
            var valoresDocente = await _reporteService.ExportarValoresEncuestaAlumno(encuestaId);

            if (valoresDocente == null)
                return NotFound("No se encontraron resultados para la encuesta especificada.");

            return Ok(valoresDocente);
        }

        [HttpGet("GenerarPdfEncuestaAlumno/{id}")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerarPdfEncuestaAlumno(int id)
        {
            try
            {
                var pdfBytes = await _reporteService.GenerarPdfEncuestaAlumno(id);
                return File(pdfBytes, "application/pdf", $"encuesta_alumno_{id}_{DateTime.Now}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar PDF: {ex.Message}" });
            }
        }
        [HttpGet("GenerarPdfEncuestaDocente/{id}")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerarPdfEncuestaDocente(int id)
        {
            try
            {
                var pdfBytes = await _reporteService.GenerarPdfEncuestaDocente(id);
                return File(pdfBytes, "application/pdf", $"encuesta_docente_{id}_{DateTime.Now}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar PDF: {ex.Message}" });
            }
        }

        [HttpGet("GenerarPdfEncuestaAsesor/{id}")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerarPdfEncuestaAsesor(int id)
        {
            try
            {
                var pdfBytes = await _reporteService.GenerarPdfEncuestaAsesor(id);
                return File(pdfBytes, "application/pdf", $"encuesta_asesor_{id}_{DateTime.Now}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar PDF: {ex.Message}" });
            }
        }

        [HttpGet("GenerarExcelEncuesta/{id}")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
