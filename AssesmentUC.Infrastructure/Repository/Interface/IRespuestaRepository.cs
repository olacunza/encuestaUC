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
        public Task<List<RespuestaEncuesta>> ListarEncuestasRespondidasRepository(string alumnoId);
        public Task RegistrarRespuestaRepository(RespuestaEncuesta respuestaModel);
        public Task ActualizarEncuestaCompletadaRepository(int encuestaId, string alumnoId);
        public Task<Encuesta> ListaPreguntasEncuestaRepository(int encuestaId);
        public Task<string> BuscarNombreDocente(string dniDocente);
        public Task<List<Encuesta>> ListaEncuestaAsignaturaPendienteRepository(string alumnoId);
        public Task<bool> VerificarSiRespondioRepository(int encuestaId, string alumnoId);
        public Task<bool> VerificarEncuestaActivaRepository(int encuestaId);
    }
}
