using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Model.Entity;
using static AssesmentUC.Model.Entity.ReporteEncuestas;

namespace AssesmentUC.Infrastructure.Repository.Interface
{
    public interface IReporteRepository
    {
        //public Task<Encuesta> ListarEncuestaRepository();
        public Task<ReporteEncuesta> ExportarValoresEncuestaAlumno(int encuestaId);
        public Task<ReporteEncuesta> ExportarValoresEncuestaDocente(int encuestaId);
        public Task<ReporteEncuesta> ExportarValoresEncuestaAsesor(int encuestaId);
    }
}
