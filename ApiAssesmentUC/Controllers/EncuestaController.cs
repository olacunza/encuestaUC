using Microsoft.AspNetCore.Mvc;
using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Service.DTO.Encuesta;

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
        public async Task<IActionResult> ListarEncuestasAsync()
        {
            var encuestas = await _encuestaService.ListarEncuestas();
            return Ok(encuestas);
        }

        [HttpGet("ListarEncuestaId")]
        public async Task<IActionResult> ListarEncuestaIdAsync(int id)
        {
            var encuesta = await _encuestaService.ListarEncuestaId(id);
            return Ok(encuesta);
        }

        [HttpPost("CrearEncuesta")]
        public async Task<IActionResult> CrearEncuesta([FromBody] EncuestaCreateDTO dto, string usuario)
        {
            try
            {
                await _encuestaService.CrearEncuestaAsync(dto, "usuario-olacunza");
                return Ok("Encuesta creada");
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
                return BadRequest("El ID enviado no coincide con el DTO");

            try
            {
                await _encuestaService.EditarEncuestaAsync(dto, "usuario-olacunza");
                return Ok("Encuesta editada correctamente");
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
            await _encuestaService.EliminarEncuestaAsync(id, "usuario-olacunza");
            return Ok("Encuesta Eliminada");
        }

        [HttpDelete("EliminarBloque/{id}")]
        public async Task<IActionResult> EliminarBloqueAsync(int id, string usuario)
        {
            await _encuestaService.EliminarBloqueAsync(id, "usuario-olacunza");
            return Ok("Bloque Eliminado");
        }

        [HttpDelete("EliminarPregunta/{id}")]
        public async Task<IActionResult> EliminarPreguntaAsync(int id, string usuario)
        {
            await _encuestaService.EliminarPreguntaAsync(id, "usuario-olacunza");
            return Ok("Pregunta Eliminada");
        }

    }
}
