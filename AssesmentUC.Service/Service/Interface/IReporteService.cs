using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Service.DTO.Encuesta;
using static AssesmentUC.Service.DTO.Reporte.ReporteEncuestaDTO;

namespace AssesmentUC.Service.Service.Interface
{
    public interface IReporteService
    {
        public Task<ReporteEncuestaDto> ExportarValoresEncuestaAlumno(int encuestaId);
        public Task<ReporteEncuestaDto> ExportarValoresEncuestaDocente(int encuestaId);
        public Task<ReporteEncuestaDto> ExportarValoresEncuestaAsesor(int encuestaId);
        //public Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId);
        //public Task<byte[]> GenerarPdfEncuesta(int id);
        public Task<byte[]> GenerarPdfEncuestaAlumno(int id);
        public Task<byte[]> GenerarPdfEncuestaDocente(int id);
        public Task<byte[]> GenerarPdfEncuestaAsesor(int id);
        //public Task<byte[]> GenerarExcelEncuesta(int id);
    }
}
