using AssesmentUC.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Infrastructure.Repository.Interface
{
    public interface IRespuestaRepository
    {
        public Task<string> ListarEncuestaRespuestaRepository();
        public Task RegistrarRespuestaRepository(RespuestaEncuesta respuestaModel);
        public Task<bool> VerificarSiRespondioRepository(int encuestaId, string alumnoId);
        public Task<bool> VerificarEncuestaActivaRepository(int encuestaId);
    }
}
