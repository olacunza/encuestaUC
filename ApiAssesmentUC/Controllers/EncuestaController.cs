using Microsoft.AspNetCore.Mvc;
using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Service.DTO.Encuesta;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Azure.Core;

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

        [HttpGet("ListarPlantillasEncuestas")]
        public async Task<IActionResult> ListarPlantillasEncuestasAsync(int pageNumber = 1, int pageSize = 10)
        {
            var encuestas = await _encuestaService.ListarPlantillasEncuestasAsync(pageNumber, pageSize);
            return Ok(encuestas);
        }

        [HttpGet("ListarAsignaturaEncuestas")]
        public async Task<IActionResult> ListarAsignaturaEncuestasAsync([FromQuery] EncuestaAsignaturaFiltroDTO dto)
        {
            var encuestas = await _encuestaService.ListarAsignaturaEncuestasAsync(dto);
            return Ok(encuestas);
        }

        [HttpGet("ListarPlantillaEncuestaId")]
        public async Task<IActionResult> ListarPlantillaEncuestaIdAsync(int id)
        {
            try
            {
                var encuesta = await _encuestaService.ListarPlantillaEncuestaIdAsync(id);
                return Ok(encuesta);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarTipoEncuesta")]
        public async Task<IActionResult> ListarTipoEncuestaAsync()
        {
            var tipoEncuesta = await _encuestaService.ListarTipoEncuestaAsync();
            return Ok(tipoEncuesta);
        }

        [HttpGet("ListarSedes")]
        public async Task<IActionResult> ListarSedesAsync()
        {
            var sedes = await _encuestaService.ListarSedesAsync();
            return Ok(sedes);
        }

        [HttpGet("ListarPeriodos")]
        public async Task<IActionResult> ListarPeriodosAsync()
        {
            var periodos = await _encuestaService.ListarPeriodosAsync();
            return Ok(periodos);
        }

        [HttpGet("ListarSecciones")]
        public async Task<IActionResult> ListarSeccionesAsync()
        {
            var secciones = await _encuestaService.ListarSeccionesAsync();
            return Ok(secciones);
        }

        [HttpGet("ListarTipoPrograma")]
        public async Task<IActionResult> ListarTipoProgramaAsync(string seccion)
        {
            var tipoPrograma = await _encuestaService.ListarTipoProgramaAsync(seccion);
            return Ok(tipoPrograma);
        }

        [HttpGet("ListarAsignaturas")]
        public async Task<IActionResult> ListarAsignaturasAsync(string seccion, string? programa = "")
        {
            var asignaturas = await _encuestaService.ListarAsignaturasAsync(seccion, programa);
            return Ok(asignaturas);
        }

        [HttpGet("ListarDocentes")]
        public async Task<IActionResult> ListarDocentesAsync(string seccion, string asignatura)
        {
            var docentes = await _encuestaService.ListarDocentesAsync(seccion, asignatura);
            return Ok(docentes);
        }

        [HttpGet("ListarTipoEncuestado")]
        public async Task<IActionResult> ListarTipoEncuestadoAsync()
        {
            var tipoEncuestado = await _encuestaService.ListarTipoEncuestadoAsync();
            return Ok(tipoEncuestado);
        }

        [HttpPost("CrearEncuestaAsignatura")]
        public async Task<IActionResult> CrearAsignaturaEncuestaAsync([FromBody] CrearEncuestaAsignaturaRequestDTO dto)
        {
            try
            {
                await _encuestaService.CrearAsignaturaEncuestaAsync(dto);
                return Ok(new { success = true, message = "Encuesta enviada por asignatura" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message});
            }
        }

        [HttpPost("CrearEncuestaPlantilla")]
        public async Task<IActionResult> CrearPlantillaEncuestaAsync([FromBody] EncuestaPlantillaCreateDTO dto, string usuario)
        {
            try
            {
                await _encuestaService.CrearPlantillaEncuestaAsync(dto, usuario);
                return Ok(new { success = true, message = "Plantilla de Encuesta creada" });
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
        public async Task<IActionResult> EditarEncuestaPlantillaAsync(int id, [FromBody] EncuestaPlantillaUpdateDTO dto, string usuario)
        {
            if ( id != dto.EncuestaId )
                return BadRequest(new { success = false, message = "El ID enviado no coincide con el DTO" });

            try
            {
                await _encuestaService.EditarEncuestaPlantillaAsync(dto, usuario);
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
    }
}
