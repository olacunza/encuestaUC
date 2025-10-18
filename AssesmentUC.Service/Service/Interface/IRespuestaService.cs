using AssesmentUC.Model.Entity;
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
        public Task<List<RespuestaListAllDTO>> ListarEncuestasRespondidasAsync(string alumnoId);
        public Task RegistrarRespuestaAsync(RespuestaEncuestaCreateDTO dto);
        public Task<bool> VerificarSiRespondioAsync(int encuestaId, string AlumnoId);
        public Task<EncuestaAsignaturaDetailDTO> ListarPreguntasEncuestaAsync(int encuestaId, string encuestadoDNI);
        public Task<List<EncuestasPendientesDTO>> ListaEncuestaAsignaturaAsync(string alumnoId);
        public Task<bool> VerificarEncuestaActivaAsync(int encuestaId);
    }
}
