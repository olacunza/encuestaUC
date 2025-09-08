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
        public Task<List<EncuestaListAllDTO>> ListarEncuestas(int pageNumber, int pageSize);
        public Task<EncuestaDetailDTO> ListarEncuestaId(int id);
        public Task<List<ListaTiposDTO>> ListarTipoEncuesta();
        public Task<List<ListaTiposDTO>> ListarSedes();
        public Task<List<ListaTiposDTO>> ListarPeriodos();
        public Task<List<ListaTiposDTO>> ListarSecciones();
        public Task<List<ListaTiposDTO>> ListarTipoPrograma();
        //public Task<> FiltrarCabeceraEncuestaAsync();
        public Task<int> CrearEncuestaAsync(EncuestaCreateDTO dto, string usuario);
        public Task EditarEncuestaAsync(EncuestaUpdateDTO dto, string usuario);
        public Task EliminarEncuestaAsync(int id, string usuario);
        public Task EliminarBloqueAsync(int id, string usuario);
        public Task EliminarPreguntaAsync(int id, string usuario);
        public Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId);
        public Task<byte[]> GenerarPdfEncuesta(int id);
        public Task<byte[]> GenerarExcelEncuesta(int id);
        public Task EnviarCorreoEncuestaAsync(string accessToken, string userEmail, int encuestaId);
    }
}
