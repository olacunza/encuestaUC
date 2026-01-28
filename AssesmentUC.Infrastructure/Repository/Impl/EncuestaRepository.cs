using AssesmentUC.Infrastructure.Data;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class EncuestaRepository : IEncuestaRepository
    {
        private readonly AppDbContext _sqlContext;
        private readonly BannerDbContext _bannerContext;

        public EncuestaRepository(AppDbContext sqlContext, BannerDbContext bannerContext)
        {
            _sqlContext = sqlContext;
            _bannerContext = bannerContext;
        }

        public async Task<List<Encuesta>> ListarPlantillaEncuestasRepository(int pageNumber, int pageSize)
        {
            return await _sqlContext.Database.SqlQuery<Encuesta>(
                $"EXEC ENC.sp_ListarPlantillaEncuestas_UC @PageNumber = {pageNumber}, @PageSize = {pageSize}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsignaturaEncuestasRepository(Encuesta filtro, int pageNumber, int pageSize)
        {
            var seccionParam = string.IsNullOrEmpty(filtro.Seccion) ? "NULL" : $"'{filtro.Seccion}'";
            var moduloParam = string.IsNullOrEmpty(filtro.Modulo) ? "NULL" : $"'{filtro.Modulo}'";
            var docenteParam = string.IsNullOrEmpty(filtro.Docente) ? "NULL" : $"'{filtro.Docente}'";
            var fechaInicioParam = filtro.FechaInicio == DateTime.MinValue ? "NULL" : $"'{filtro.FechaInicio:yyyy-MM-dd}'";
            var fechaFinParam = filtro.FechaFin == DateTime.MinValue ? "NULL" : $"'{filtro.FechaFin:yyyy-MM-dd}'";

            var sql = $@"
                EXEC ENC.sp_ListarAsignaturaEncuestas_UC 
                    @SECCION = {seccionParam},
                    @MODULO = {moduloParam},
                    @DOCENTE = {docenteParam},
                    @FECHA_INICIO = {fechaInicioParam},
                    @FECHA_FIN = {fechaFinParam},
                    @PAGENUMBER = {pageNumber},
                    @PAGESIZE = {pageSize}";

            return await _sqlContext.Database.SqlQuery<Encuesta>(sql).ToListAsync();
        }

        public async Task<Encuesta> ListarPlantillaEncuestaIdRepository(int id)
        {
            var encuesta = await _sqlContext.Database.SqlQuery<Encuesta>(
                $"EXEC ENC.sp_ListarPlantillaEncuestaId @ENCUESTA_ID = {id}"
            ).FirstOrDefaultAsync();

            if (encuesta == null)
                throw new KeyNotFoundException($"No se encontró ninguna encuesta con el ID {id}");

            return encuesta;
        }

        public async Task<List<Encuesta>> ListarTipoEncuestaRepository()
        {
            return await _sqlContext.Database.SqlQuery<Encuesta>(
                "EXEC ENC.sp_ListarTipoEncuesta_UC"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarTipoEncuestadoRepository()
        {
            return await _sqlContext.Database.SqlQuery<Encuesta>(
                "EXEC ENC.sp_ListarTipoEncuestado"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarSedesRepository()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                "EXEC BANINST1.SZKENC.P_LISTAR_SEDES"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarPeriodosRepository()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                "EXEC BANINST1.SZKENC.P_LISTAR_PERIODOS"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarSeccionesRepository()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                "EXEC BANINST1.SZKENC.P_LISTAR_SECCIONES"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsignaturasRepository(string seccion, string? programa)
        {
            var programaParam = string.IsNullOrEmpty(programa) ? "''" : $"'{programa}'";

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $"EXEC BANINST1.SZKENC.P_LISTAR_ASIGNATURAS @p_blck_code = '{seccion}', @p_program_id = {programaParam}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarDocentesRepository(string seccion, string asignatura)
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $"EXEC BANINST1.SZKENC.P_LISTAR_DOCENTES_ASIGNATURA @p_seccion = '{seccion}', @p_asignatura = '{asignatura}'"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarTipoProgramaRepository(string seccion)
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $"EXEC BANINST1.SZKENC.P_LISTAR_TIPO_PROGRAMA @P_BLCK_CODE = '{seccion}'"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsesoresRepository(string seccion)
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $"EXEC BANINST1.SZKENC.P_LISTAR_ASESORES @P_BLCK_CODE = '{seccion}'"
            ).ToListAsync();
        }

        public async Task<int> CrearAsignaturaEncuestaRepository(Encuesta encuesta)
        {
            var nombreEncuestaParam = string.IsNullOrEmpty(encuesta.NombreEncuesta) ? "NULL" : $"'{encuesta.NombreEncuesta}'";
            var descripcionParam = string.IsNullOrEmpty(encuesta.DescripcionEncuesta) ? "NULL" : $"'{encuesta.DescripcionEncuesta}'";
            var tipoEncuestaIdParam = encuesta.TipoEncuestaId.HasValue ? encuesta.TipoEncuestaId.ToString() : "NULL";
            var tipoEncuestadoIdParam = encuesta.TipoEncuestadoId.HasValue ? encuesta.TipoEncuestadoId.ToString() : "NULL";
            var tipoProgramaParam = string.IsNullOrEmpty(encuesta.TipoPrograma) ? "NULL" : $"'{encuesta.TipoPrograma}'";
            var sedeParam = string.IsNullOrEmpty(encuesta.Sede) ? "NULL" : $"'{encuesta.Sede}'";
            var periodoIdParam = string.IsNullOrEmpty(encuesta.PeriodoId) ? "NULL" : $"'{encuesta.PeriodoId}'";
            var seccionIdParam = string.IsNullOrEmpty(encuesta.SeccionId) ? "NULL" : $"'{encuesta.SeccionId}'";
            var moduloParam = string.IsNullOrEmpty(encuesta.Modulo) ? "NULL" : $"'{encuesta.Modulo}'";
            var docenteParam = string.IsNullOrEmpty(encuesta.Docente) ? "NULL" : $"'{encuesta.Docente}'";
            var fechaInicioParam = encuesta.FechaInicio == DateTime.MinValue ? "NULL" : $"'{encuesta.FechaInicio:yyyy-MM-dd}'";
            var fechaFinParam = encuesta.FechaFin == DateTime.MinValue ? "NULL" : $"'{encuesta.FechaFin:yyyy-MM-dd}'";
            var activoParam = encuesta.Activo.HasValue ? (encuesta.Activo.Value ? "1" : "0") : "NULL";
            var usuarioParam = string.IsNullOrEmpty(encuesta.UsuarioCreacion) ? "NULL" : $"'{encuesta.UsuarioCreacion}'";

            var sql = $@"
                EXEC ENC.sp_CrearEncuestaAsignatura_UC 
                    @NOMBRE_ENCUESTA = {nombreEncuestaParam},
                    @DESCRIPCION_ENCUESTA = {descripcionParam},
                    @TIPO_ENCUESTA_ID = {tipoEncuestaIdParam},
                    @TIPO_ENCUESTADO_ID = {tipoEncuestadoIdParam},
                    @TIPO_PROGRAMA = {tipoProgramaParam},
                    @SEDE = {sedeParam},
                    @PERIODO_ID = {periodoIdParam},
                    @SECCION_ID = {seccionIdParam},
                    @MODULO = {moduloParam},
                    @DOCENTE = {docenteParam},
                    @FECHA_INICIO = {fechaInicioParam},
                    @FECHA_FIN = {fechaFinParam},
                    @ACTIVO = {activoParam},
                    @FECHA_ENVIO = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}',
                    @USUARIO_ENVIO = {usuarioParam}";

            return await _sqlContext.Database.SqlQuery<int>(sql).FirstOrDefaultAsync();
        }

        public async Task<List<Encuesta>> ListarAlumnosRepository(string seccion, string asignatura)
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $"EXEC BANINST1.SZKENC.P_LISTAR_CORREOS_ENCUESTA @p_asignatura = '{asignatura}', @p_seccion = '{seccion}'"
            ).ToListAsync();
        }

        public async Task InsertarEncuestasPorAsignaturaBulkAsync(int encuestaId, string usuario, List<string> alumnos, int tipoEncuestadoId)
        {
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                if (connection is SqlConnection sqlConn)
                {
                    var table = new System.Data.DataTable();
                    table.Columns.Add("ENCUESTA_ASIGNATURA_ID", typeof(int));
                    table.Columns.Add("DNI_ENCUESTADO", typeof(string));
                    table.Columns.Add("COMPLETADO", typeof(bool));
                    table.Columns.Add("FECHA_CREACION", typeof(DateTime));
                    table.Columns.Add("USUARIO_CREACION", typeof(string));
                    table.Columns.Add("TIPO_ENCUESTADO_ID", typeof(int));

                    foreach (var alumnoId in alumnos)
                    {
                        table.Rows.Add(encuestaId, alumnoId, false, DateTime.Now, usuario, tipoEncuestadoId);
                    }

                    using var bulk = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, null)
                    {
                        DestinationTableName = "ENC.tblEncuestaAsignaturaAlumno"
                    };

                    bulk.ColumnMappings.Add("ENCUESTA_ASIGNATURA_ID", "ENCUESTA_ASIGNATURA_ID");
                    bulk.ColumnMappings.Add("DNI_ENCUESTADO", "DNI_ENCUESTADO");
                    bulk.ColumnMappings.Add("COMPLETADO", "COMPLETADO");
                    bulk.ColumnMappings.Add("FECHA_CREACION", "FECHA_CREACION");
                    bulk.ColumnMappings.Add("USUARIO_CREACION", "USUARIO_CREACION");
                    bulk.ColumnMappings.Add("TIPO_ENCUESTADO_ID", "TIPO_ENCUESTADO_ID");

                    await bulk.WriteToServerAsync(table);
                }
                else
                {
                    throw new InvalidOperationException("La conexión no es de tipo SqlConnection. Bulk copy no disponible.");
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public async Task<int> CrearPlantillaEncuestaRepository(Encuesta encuesta)
        {
            var nombreEncuestaParam = string.IsNullOrEmpty(encuesta.NombreEncuesta) ? "NULL" : $"'{encuesta.NombreEncuesta}'";
            var descripcionParam = string.IsNullOrEmpty(encuesta.DescripcionEncuesta) ? "NULL" : $"'{encuesta.DescripcionEncuesta}'";
            var tipoEncuestaIdParam = encuesta.TipoEncuestaId.HasValue ? encuesta.TipoEncuestaId.ToString() : "NULL";
            var usuarioParam = string.IsNullOrEmpty(encuesta.UsuarioCreacion) ? "NULL" : $"'{encuesta.UsuarioCreacion}'";

            var sql = $@"
                EXEC ENC.sp_CrearEncuestaPlantilla_UC 
                    @NOMBRE_ENCUESTA = {nombreEncuestaParam},
                    @DESCRIPCION_ENCUESTA = {descripcionParam},
                    @TIPO_ENCUESTA_ID = {tipoEncuestaIdParam},
                    @FECHA_CREACION = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}',
                    @USUARIO_CREACION = {usuarioParam}";

            return await _sqlContext.Database.SqlQuery<int>(sql).FirstOrDefaultAsync();
        }

        public async Task EditarEncuestaPlantillaRepository(Encuesta encuesta)
        {
            var nombreEncuestaParam = string.IsNullOrEmpty(encuesta.NombreEncuesta) ? "NULL" : $"'{encuesta.NombreEncuesta}'";
            var descripcionParam = string.IsNullOrEmpty(encuesta.DescripcionEncuesta) ? "NULL" : $"'{encuesta.DescripcionEncuesta}'";
            var tipoEncuestaParam = encuesta.TipoEncuestaId.HasValue ? encuesta.TipoEncuestaId.ToString() : "NULL";
            var usuarioParam = string.IsNullOrEmpty(encuesta.UsuarioModificacion) ? "NULL" : $"'{encuesta.UsuarioModificacion}'";

            var sql = $@"
                EXEC ENC.sp_EditarPlantillaEncuesta_UC 
                    @ENCUESTA_ID = {encuesta.EncuestaId},
                    @NOMBRE_ENCUESTA = {nombreEncuestaParam},
                    @DESCRIPCION_ENCUESTA = {descripcionParam},
                    @TIPO_ENCUESTA = {tipoEncuestaParam},
                    @USUARIO_MODIFICACION = {usuarioParam},
                    @FECHA_MODIFICACION = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

            await _sqlContext.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task EliminarEncuestaRepository(int id, string usuario)
        {
            var sql = $@"
                EXEC ENC.sp_EliminarEncuestaPlantilla_UC 
                    @ENCUESTA_ID = {id},
                    @USUARIO_MODIFICACION = '{usuario}',
                    @FECHA_MODIFICACION = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

            await _sqlContext.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task EliminarBloqueRepository(int id, string usuario)
        {
            var sql = $@"
                EXEC ENC.sp_EliminarBloquePlantilla_UC 
                    @BLOQUE_ID = {id},
                    @USUARIO_MODIFICACION = '{usuario}',
                    @FECHA_MODIFICACION = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

            await _sqlContext.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task EliminarPreguntaRepository(int id, string usuario)
        {
            var sql = $@"
                EXEC ENC.sp_EliminarPreguntaPlantilla_UC 
                    @PREGUNTA_ID = {id},
                    @USUARIO_MODIFICACION = '{usuario}',
                    @FECHA_MODIFICACION = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

            await _sqlContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}