using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Model.Entity;

namespace AssesmentUC.Infrastructure.Repository.Interface
{
    public interface IReporteRepository
    {
        public Task<Encuesta> ListarEncuestaRepository();
    }
}
