using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Respuesta;
using AssesmentUC.Service.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.Service.Impl
{
    public class RespuestaService : IRespuestaService
    {
        public readonly IRespuestaRepository _respuestaRepository;

        public RespuestaService(IRespuestaRepository respuestaRepository)
        {
            _respuestaRepository = respuestaRepository;
        }

        public async Task<string> ListarEncuestaRespuestaAsync()
        {
            var rpta = await _respuestaRepository.ListarEncuestaRespuestaRepository();
            return rpta;
        }

        public async Task RegistrarRespuestaAsync(RespuestaEncuestaCreateDTO dto)
        {
            var respuestaModel = new RespuestaEncuesta
            {
                EncuestaId = dto.EncuestaId,
                AlumnoId = dto.AlumnoId,
                FechaRespuesta = DateTime.Now,
                Completado = dto.Completado,
                Respuestas = dto.Respuestas.Select( r => new RespuestaPregunta
                {
                    EncuestaPreguntaId = r.EncuestaPreguntaId,
                    ValorRespuesta = r.ValorRespuesta
                }).ToList()
            };

            await _respuestaRepository.RegistrarRespuestaRepository(respuestaModel);
        }

        public async Task<bool> VerificarSiRespondioAsync(int encuestaId, string alumnoId)
        {
            return await _respuestaRepository.VerificarSiRespondioRepository(encuestaId, alumnoId);
        }

        public async Task<bool> VerificarEncuestaActivaAsync(int encuestaId)
        {
            return await _respuestaRepository.VerificarEncuestaActivaRepository(encuestaId);
        }

    }
}
