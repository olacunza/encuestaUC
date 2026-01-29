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
        public Task<List<Encuesta>> ListarPlantillaEncuestas(int pageNumber, int pageSize);
        public Task<List<Encuesta>> ListarAsignaturaEncuestas(Encuesta filtro, int pageNumber, int pageSize);
        public Task<Encuesta> ListarPlantillaEncuestaById(int id);
        public Task<List<Encuesta>> ListarTipoEncuesta();
        public Task<List<Encuesta>> ListarSedes();
        public Task<List<Encuesta>> ListarPeriodos();
        public Task<List<Encuesta>> ListarSecciones();
        public Task<List<Encuesta>> ListarAsignaturas(string seccion, string? programa);
        public Task<List<Encuesta>> ListarDocentes(string seccion, string asignatura);
        public Task<List<Encuesta>> ListarTipoPrograma(string seccion);
        public Task<List<Encuesta>> ListarAsesores(string seccion);
        public Task<List<Encuesta>> ListarTipoEncuestado();
        public Task<int> CrearAsignaturaEncuesta(Encuesta encuesta);
        public Task InsertarEncuestasPorAsignaturaBulkAsync(int encuestaId, string usuario, List<string> alumnos, int TipoEncuestadoId);
        public Task<int> CrearPlantillaEncuesta(Encuesta encuesta);
        public Task EditarEncuestaPlantilla(Encuesta encuesta);
        public Task EliminarEncuesta(int id, string usuario);
        public Task EliminarBloque(int id, string usuario);
        public Task EliminarPregunta(int id, string usuario);
        public Task<List<Encuesta>> ListarAlumnos(string seccion, string asignatura);
    }
}
