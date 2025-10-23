using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Respuesta;
using AssesmentUC.Service.Service.Impl;
using AssesmentUC.Service.Service.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
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

        [HttpGet("ListarEncuestasRespondidas/{alumnoID}")]
        public async Task<IActionResult> ListarEncuestasRespondidasAsync(string alumnoID)
        {
            var encuesta = await _respuestaService.ListarEncuestasRespondidasAsync(alumnoID);
            return Ok(encuesta);
        }

        [HttpPost("RegistrarRespuesta")]
        public async Task<IActionResult> RegistrarRespuestaAsync([FromBody] RespuestaEncuestaCreateDTO dto)
        {
            try
            {
                var encuestaRespondida = await _respuestaService.VerificarSiRespondioAsync(dto.EncuestaId, dto.AlumnoId);
                var encuestaActiva = await _respuestaService.VerificarEncuestaActivaAsync(dto.EncuestaId);
             
                if (!encuestaRespondida && encuestaActiva)
                {
                    await _respuestaService.RegistrarRespuestaAsync(dto);
                    return Ok(new { success = true, message = "Respuestas guardadas correctamente" });

                }
                else
                {
                    return Ok(new { success = false, message = "El alumno ya ha respondido la encuesta" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { success = false, message = "Error al registrar la respuesta" });
            }
        }

        [HttpGet("VerificarEncuestaRespondida/{encuestaId}/{alumnoId}")]
        public async Task<IActionResult> VerificarSiRespondioAsync(int encuestaId, string alumnoId)
        {
            bool respondido = await _respuestaService.VerificarSiRespondioAsync(encuestaId, alumnoId);
            return Ok(respondido);
        }

        [HttpGet("ListarPreguntasEncuesta/{encuestaId}/{encuestadoDNI}")]
        public async Task<IActionResult> ListarPreguntasEncuestaAsignaturaAsync(int encuestaId, string encuestadoDNI)
        {
            var encuestaRespondida = await _respuestaService.VerificarSiRespondioAsync(encuestaId, encuestadoDNI);
            var encuestaActiva = await _respuestaService.VerificarEncuestaActivaAsync(encuestaId);

            if (!encuestaRespondida && encuestaActiva)
            {
                var encuesta = await _respuestaService.ListarPreguntasEncuestaAsync(encuestaId, encuestadoDNI);
                if (encuesta == null)
                {
                    return NotFound(new { message = $"No se encontró ninguna encuesta con el ID {encuestaId}" });
                }
                return Ok(encuesta);
            }
            else
            {
                return Ok(new { success = false, message = $"El usuario ya ha respondido la encuesta con ID {encuestaId}" });
            }
            
        }

        [HttpGet("ListaEncuestasPendientes/{alumnoId}")]
        public async Task<IActionResult> ListaEncuestaAsignaturaAsync(string alumnoId)
        {
            var encuesta = await _respuestaService.ListaEncuestaAsignaturaAsync(alumnoId);
            if (encuesta == null)
                return NotFound("No tienes encuestas pendientes");

            return Ok(encuesta);
        }


    }
}
