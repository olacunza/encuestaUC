using AssesmentUC.Service.Service.Impl;
using AssesmentUC.Service.Service.Interface;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using AssesmentUC.Service.DTO.Encuesta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssesmentUC.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    [Produces("application/json")]
    public class EncuestaController : ControllerBase
    {
        private readonly IEncuestaService _encuestaService;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<EncuestaController> _logger;

        public EncuestaController(
            IEncuestaService encuestaService,
            IAuthenticationService authService,
            ILogger<EncuestaController> logger)
        {
            _encuestaService = encuestaService;
            _authService = authService;
            _logger = logger;
        }

        private string? GetCurrentUserEmail()
        {
            return _authService.GetUserEmail(User);
        }

        [HttpGet("ListarPlantillasEncuestas")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPlantillasEncuestasAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} accedió a ListarPlantillasEncuestas", userEmail);

                var encuestas = await _encuestaService.ListarPlantillasEncuestasAsync(pageNumber, pageSize);
                return Ok(encuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar plantillas de encuestas");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarAsignaturaEncuestas")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarAsignaturaEncuestasAsync(
            [FromQuery] EncuestaAsignaturaFiltroDTO dto)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} filtró encuestas por asignatura", userEmail);

                var encuestas = await _encuestaService.ListarAsignaturaEncuestasAsync(dto);
                return Ok(encuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar encuestas por asignatura");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarPlantillaEncuestaId")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarPlantillaEncuestaIdAsync(int id)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} consultó encuesta ID {EncuestaId}", userEmail, id);

                var encuesta = await _encuestaService.ListarPlantillaEncuestaIdAsync(id);
                return Ok(encuesta);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Encuesta no encontrada: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener encuesta");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarTipoEncuesta")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarTipoEncuestaAsync()
        {
            try
            {
                var tipoEncuesta = await _encuestaService.ListarTipoEncuestaAsync();
                return Ok(tipoEncuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tipos de encuesta");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarSedes")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarSedesAsync()
        {
            try
            {
                var sedes = await _encuestaService.ListarSedesAsync();
                return Ok(sedes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar sedes");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarPeriodos")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarPeriodosAsync()
        {
            try
            {
                var periodos = await _encuestaService.ListarPeriodosAsync();
                return Ok(periodos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar períodos");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarSecciones")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarSeccionesAsync()
        {
            try
            {
                var secciones = await _encuestaService.ListarSeccionesAsync();
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar secciones");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarTipoPrograma")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarTipoProgramaAsync(string seccion)
        {
            try
            {
                var tipoPrograma = await _encuestaService.ListarTipoProgramaAsync(seccion);
                return Ok(tipoPrograma);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tipos de programa");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarAsignaturas")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarAsignaturasAsync(
            string seccion,
            string? programa = "")
        {
            try
            {
                var asignaturas = await _encuestaService.ListarAsignaturasAsync(seccion, programa);
                return Ok(asignaturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar asignaturas");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarDocentes")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarDocentesAsync(string seccion, string asignatura)
        {
            try
            {
                var docentes = await _encuestaService.ListarDocentesAsync(seccion, asignatura);
                return Ok(docentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar docentes");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarAsesores")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarAsesoresAsync(string seccion)
        {
            try
            {
                var asesores = await _encuestaService.ListarAsesoresAsync(seccion);
                return Ok(asesores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar asesores");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("ListarTipoEncuestado")]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarTipoEncuestadoAsync()
        {
            try
            {
                var tipoEncuestado = await _encuestaService.ListarTipoEncuestadoAsync();
                return Ok(tipoEncuestado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tipos de encuestado");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPost("CrearEncuestaAsignatura")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearAsignaturaEncuestaAsync(
            [FromBody] CrearEncuestaAsignaturaRequestDTO dto)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} creó encuesta de asignatura", userEmail);

                await _encuestaService.CrearAsignaturaEncuestaAsync(dto);
                return Ok(new { success = true, message = "Encuesta enviada por asignatura", usuario = userEmail });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear encuesta de asignatura");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPost("CrearEncuestaPlantilla")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearPlantillaEncuestaAsync(
            [FromBody] EncuestaPlantillaCreateDTO dto,
            string usuario)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} creó plantilla de encuesta", userEmail);

                await _encuestaService.CrearPlantillaEncuestaAsync(dto, usuario);
                return Ok(new { success = true, message = "Plantilla de Encuesta creada", usuario = userEmail });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear plantilla");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPut("EditarEncuesta/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditarEncuestaPlantillaAsync(
            int id,
            [FromBody] EncuestaPlantillaUpdateDTO dto,
            string usuario)
        {
            if (id != dto.EncuestaId)
                return BadRequest(new { success = false, message = "El ID enviado no coincide con el DTO" });

            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} editó encuesta ID {EncuestaId}", userEmail, id);

                await _encuestaService.EditarEncuestaPlantillaAsync(dto, usuario);
                return Ok(new { success = true, message = "Encuesta editada correctamente", usuario = userEmail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar encuesta");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpDelete("EliminarEncuesta/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EliminarEncuestaAsync(
            int id,
            string usuario)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} eliminó encuesta ID {EncuestaId}", userEmail, id);

                await _encuestaService.EliminarEncuestaAsync(id, usuario);
                return Ok(new { success = true, message = "Encuesta eliminada", usuario = userEmail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar encuesta");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpDelete("EliminarBloque/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EliminarBloqueAsync(int id, string usuario)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} eliminó bloque ID {BloqueId}", userEmail, id);

                await _encuestaService.EliminarBloqueAsync(id, usuario);
                return Ok(new { success = true, message = "Bloque eliminado", usuario = userEmail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar bloque");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpDelete("EliminarPregunta/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EliminarPreguntaAsync(int id, string usuario)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                _logger.LogInformation("Usuario {Email} eliminó pregunta ID {PreguntaId}", userEmail, id);

                await _encuestaService.EliminarPreguntaAsync(id, usuario);
                return Ok(new { success = true, message = "Pregunta eliminada", usuario = userEmail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pregunta");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
