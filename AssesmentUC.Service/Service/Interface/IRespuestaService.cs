using AssesmentUC.Service.DTO.Respuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.Service.Interface
{
    public interface IRespuestaService
    {
        public Task<string> ListarEncuestaRespuestaAsync();
        public Task RegistrarRespuestaAsync(RespuestaEncuestaCreateDTO dto);
        public Task<bool> VerificarSiRespondioAsync(int encuestaId, string AlumnoId);
        public Task<bool> VerificarEncuestaActivaAsync(int encuestaId);
    }
}
