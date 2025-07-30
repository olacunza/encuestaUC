using AssesmentUC.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Infrastructure.Repository.Interface
{
    public interface IEncuestaRepository
    {
        public Task<List<Encuesta>> ListarEncuestasRepository();
        public Task<Encuesta> ListarEncuestaIdRepository(int id);
        public Task<int> CrearEncuestaRepository(Encuesta encuesta);
        public Task EditarEncuestaRepository(Encuesta encuesta);
        public Task EliminarEncuestaRepository(int id, string usuario);
        public Task EliminarBloqueRepository(int id, string usuario);
        public Task EliminarPreguntaRepository(int id, string usuario);
    }
}
