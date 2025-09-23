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
        public Task<List<Encuesta>> ListarPlantillaEncuestasRepository(int pageNumber, int pageSize);
        public Task<Encuesta> ListarPlantillaEncuestaIdRepository(int id);
        public Task<List<Encuesta>> ListarTipoEncuestaRepository();
        public Task<List<Encuesta>> ListarSedesRepository();
        public Task<List<Encuesta>> ListarPeriodosRepository();
        public Task<List<Encuesta>> ListarSeccionesRepository();
        public Task<List<Encuesta>> ListarAsignaturasRepository(string seccion);
        public Task<List<Encuesta>> ListarTipoProgramaRepository();
        public Task<int> CrearAsignaturaEncuestaRepository(Encuesta encuesta);
        public Task InsertarEncuestasPorAsignaturaBulkAsync(int encuestaId, string usuario, List<string> alumnos);
        public Task<Encuesta> ListarDatosAsignaturaRepository(string asignatura);
        public Task<int> CrearPlantillaEncuestaRepository(Encuesta encuesta);
        public Task EditarEncuestaPlantillaRepository(Encuesta encuesta);
        public Task EliminarEncuestaRepository(int id, string usuario);
        public Task EliminarBloqueRepository(int id, string usuario);
        public Task EliminarPreguntaRepository(int id, string usuario);
        public Task<List<AlumnoCorreo>> ListarCorreosEncuestaAsignaturaRepository(string asignatura);
        //public Task ActualizarEncuestaCompletadaRepository(int encuestaId, string userEmail);
    }
}
