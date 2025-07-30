using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Respuesta;
using AssesmentUC.Service.Service.Impl;
using AssesmentUC.Service.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentUC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class RespuestaController : ControllerBase
    {
        private readonly IRespuestaService _respuestaService;
        
        public RespuestaController(IRespuestaService respuestaService)
        {
            _respuestaService = respuestaService;
        }

        [HttpGet("ListarRespuestasEncuestas")]
        public async Task<IActionResult> ListarRespuestasEncuestasAsync()
        {
            var encuesta = await _respuestaService.ListarEncuestaRespuestaAsync();
            return Ok(encuesta);
        }

        [HttpPost("RegistrarRespuesta")]
        public async Task<IActionResult> RegistrarRespuestaAsync([FromBody] RespuestaEncuestaCreateDTO dto)
        {
            try
            {
                var encuestaRespondida = await _respuestaService.VerificarSiRespondioAsync(dto.EncuestaId, dto.AlumnoId);
                var encuestaActiva = await _respuestaService.VerificarEncuestaActivaAsync(dto.EncuestaId);
             
                if (!encuestaRespondida)
                {
                    await _respuestaService.RegistrarRespuestaAsync(dto);
                    return Ok("Respuestas guardadas correctamente");

                }
                else
                {
                    return Ok("El alumno ya ha respondido la encuesta");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Error al registrar la respuesta");
            }
        }

        [HttpGet("VerificarRespuestas/{encuestaId}/{alumnoId}")]
        public async Task<IActionResult> VerificarSiRespondioAsync(int encuestaId, string alumnoId)
        {
            bool respondido = await _respuestaService.VerificarSiRespondioAsync(encuestaId, alumnoId);
            return Ok(respondido);
        }

        //[HttpGet("EncuestaPendiente/{id}")]
        //public async Task<IActionResult> EncuestaPendienteAsync(int id)
        //{
        //    var encuesta = await _encuestaService.EncuestaPendienteAsync(id);
        //    if (encuesta == null)
        //        return NotFound("Encuesta no encontrada");

        //    return Ok(encuesta);
        //}


    }
}
