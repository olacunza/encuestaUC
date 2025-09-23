using AssesmentUC.Service.DTO.Encuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.Service.Interface
{
    public interface IEncuestaService
    {
        public Task<List<EncuestaListAllDTO>> ListarPlantillasEncuestasAsync(int pageNumber, int pageSize);
        public Task<EncuestaPlantillaDetailDTO> ListarPlantillaEncuestaIdAsync(int id);
        public Task<List<ListaTiposDTO>> ListarTipoEncuestaAsync();
        public Task<List<ListaTiposDTO>> ListarSedesAsync();
        public Task<List<ListaTiposDTO>> ListarPeriodosAsync();
        public Task<List<ListaTiposDTO>> ListarSeccionesAsync();
        public Task<List<ListaTiposDTO>> ListarAsignaturasAsync(string seccion);
        public Task<List<ListaTiposDTO>> ListarTipoProgramaAsync();
        public Task CrearAsignaturaEncuestaAsync(CrearEncuestaAsignaturaRequestDTO dto);
        public Task<int> CrearPlantillaEncuestaAsync(EncuestaPlantillaCreateDTO dto, string usuario);
        public Task EditarEncuestaPlantillaAsync(EncuestaPlantillaUpdateDTO dto, string usuario);
        public Task EliminarEncuestaAsync(int id, string usuario);
        public Task EliminarBloqueAsync(int id, string usuario);
        public Task EliminarPreguntaAsync(int id, string usuario);
        public Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId);
        public Task<byte[]> GenerarPdfEncuesta(int id);
        public Task<byte[]> GenerarExcelEncuesta(int id);
        //public Task EnviarCorreoEncuestaAsync(string accessToken, string userEmail, int encuestaId);
    }
}
