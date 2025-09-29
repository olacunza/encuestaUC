using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Service.DTO.Encuesta;

namespace AssesmentUC.Service.Service.Interface
{
    public interface IReporteService
    {
        public Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId);
        public Task<byte[]> GenerarPdfEncuesta(int id);
        public Task<byte[]> GenerarExcelEncuesta(int id);
    }
}
