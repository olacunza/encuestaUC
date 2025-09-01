using Microsoft.AspNetCore.Mvc;
using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Service.DTO.Encuesta;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace AssesmentUC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class EncuestaController : ControllerBase
    {
        private readonly IEncuestaService _encuestaService;
        public EncuestaController(IEncuestaService encuestaService)
        {
            _encuestaService = encuestaService;
        }

        [HttpGet("ListarEncuestas")]
        public async Task<IActionResult> ListarEncuestasAsync(int pageNumber = 1, int pageSize = 10)
        {
            var encuestas = await _encuestaService.ListarEncuestas(pageNumber, pageSize);
            return Ok(encuestas);
        }

        [HttpGet("ListarEncuestaId")]
        public async Task<IActionResult> ListarEncuestaIdAsync(int id)
        {
            var encuesta = await _encuestaService.ListarEncuestaId(id);
            return Ok(encuesta);
        }

        [HttpGet("ListarTipoEncuesta")]
        public async Task<IActionResult> ListarTipoEncuestaAsync()
        {
            var tipoEncuesta = await _encuestaService.ListarTipoEncuesta();
            return Ok(tipoEncuesta);
        }

        [HttpGet("ListarSedes")]
        public async Task<IActionResult> ListarSedesAsync()
        {
            var sedes = await _encuestaService.ListarSedes();
            return Ok(sedes);
        }

        [HttpGet("ListarPeriodos")]
        public async Task<IActionResult> ListarPeriodosAsync()
        {
            var periodos = await _encuestaService.ListarPeriodos();
            return Ok(periodos);
        }

        [HttpGet("ListarSecciones")]
        public async Task<IActionResult> ListarSeccionesAsync()
        {
            var secciones = await _encuestaService.ListarSecciones();
            return Ok(secciones);
        }

        [HttpGet("ListarTipoPrograma")]
        public async Task<IActionResult> ListarTipoProgramaAsync()
        {
            var tipoPrograma = await _encuestaService.ListarTipoPrograma();
            return Ok(tipoPrograma);
        }

        //[HttpGet("FiltrarCabeceraEncuesta")]
        //public async Task<IActionResult> FiltrarCabeceraEncuestaAsync()
        //{
        //    var filtrarCabecera = await _encuestaService.FiltrarCabeceraEncuesta();
        //    return Ok(filtrarCabecera);
        //}

        [HttpPost("CrearEncuesta")]
        public async Task<IActionResult> CrearEncuesta([FromBody] EncuestaCreateDTO dto, string usuario)
        {
            try
            {
                await _encuestaService.CrearEncuestaAsync(dto, usuario);
                return Ok(new { success = true, message = "Encuesta creada" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "ERror interno del servidor", detalle = ex.Message});
            }
        }

        [HttpPut("EditarEncuesta/{id}")]
        public async Task<IActionResult> EditarEncuestaAsync(int id, [FromBody] EncuestaUpdateDTO dto, string usuario)
        {
            if ( id != dto.EncuestaId )
                return BadRequest(new { success = false, message = "El ID enviado no coincide con el DTO" });

            try
            {
                await _encuestaService.EditarEncuestaAsync(dto, usuario);
                return Ok(new { success = true, message = "Encuesta editada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpDelete("EliminarEncuesta/{id}")]
        public async Task<IActionResult> EliminarEncuestaAsync(int id, string usuario)
        {
            await _encuestaService.EliminarEncuestaAsync(id, usuario);
            return Ok(new { success = true, message = "Encuesta eliminada" });
        }

        [HttpDelete("EliminarBloque/{id}")]
        public async Task<IActionResult> EliminarBloqueAsync(int id, string usuario)
        {
            await _encuestaService.EliminarBloqueAsync(id, usuario);
            return Ok(new { success = true, message = "Bloque eliminado" });
        }

        [HttpDelete("EliminarPregunta/{id}")]
        public async Task<IActionResult> EliminarPreguntaAsync(int id, string usuario)
        {
            await _encuestaService.EliminarPreguntaAsync(id, usuario);
            return Ok(new { success = true, message = "Pregunta eliminada" });
        }

        [HttpGet("GenerarPdfEncuesta/{id}")]
        public async Task<IActionResult> GenerarPdfEncuesta(int id)
        {
            try
            {
                var pdfBytes = await _encuestaService.GenerarPdfEncuesta(id);
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
                var excelBytes = await _encuestaService.GenerarExcelEncuesta(id);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"encuesta_{id}_{DateTime.Now}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al generar Excel: {ex.Message}" });
            }
        }


    }
}
