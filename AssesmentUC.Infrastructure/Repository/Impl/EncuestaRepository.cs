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

        public async Task<List<Encuesta>> ListarPlantillaEncuestas(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "El número de página debe ser mayor que 0");
            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "El tamaño de página debe estar entre 1 y 100");

            return await _sqlContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_PLANTILLA_ENCUESTAS}
                   @PageNumber = {pageNumber},
                   @PageSize = {pageSize}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsignaturaEncuestas(Encuesta filtro, int pageNumber, int pageSize)
        {
            if (filtro == null)
                throw new ArgumentNullException(nameof(filtro));
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Parámetros de paginación inválidos");

            return await _sqlContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_ASIGNATURA_ENCUESTAS}
                    @SECCION = {filtro.Seccion},
                    @MODULO = {filtro.Modulo},
                    @DOCENTE = {filtro.Docente},
                    @FECHA_INICIO = {(filtro.FechaInicio != DateTime.MinValue ? filtro.FechaInicio : (DateTime?)null)},
                    @FECHA_FIN = {(filtro.FechaFin != DateTime.MinValue ? filtro.FechaFin : (DateTime?)null)},
                    @PAGENUMBER = {pageNumber},
                    @PAGESIZE = {pageSize}"
            ).ToListAsync();
        }

        public async Task<Encuesta> ListarPlantillaEncuestaById(int id)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que 0");

            var encuesta = await _sqlContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_PLANTILLA_ENCUESTA_ID}
                    @ENCUESTA_ID = {id}"
            ).FirstOrDefaultAsync();

            if (encuesta == null)
                throw new KeyNotFoundException($"No se encontró encuesta con ID {id}");

            return encuesta;
        }

        public async Task<List<Encuesta>> ListarTipoEncuesta()
        {
            return await _sqlContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_TIPO_ENCUESTA}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarTipoEncuestado()
        {
            return await _sqlContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_TIPO_ENCUESTADO}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarSedes()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_SEDES}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarPeriodos()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_PERIODOS}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarSecciones()
        {
            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_SECCIONES}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsignaturas(string seccion, string? programa)
        {
            if (string.IsNullOrWhiteSpace(seccion))
                throw new ArgumentException("La sección no puede estar vacía", nameof(seccion));

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_ASIGNATURAS}
                    @p_blck_code = {seccion},
                    @p_program_id = {programa ?? ""}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarDocentes(string seccion, string asignatura)
        {
            if (string.IsNullOrWhiteSpace(seccion) || string.IsNullOrWhiteSpace(asignatura))
                throw new ArgumentException("La sección y asignatura no pueden estar vacías");

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_DOCENTES}
                    @p_seccion = {seccion},
                    @p_asignatura = {asignatura}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarTipoPrograma(string seccion)
        {
            if (string.IsNullOrWhiteSpace(seccion))
                throw new ArgumentException("La sección no puede estar vacía", nameof(seccion));

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_TIPO_PROGRAMA}
                    @P_BLCK_CODE = {seccion}"
            ).ToListAsync();
        }

        public async Task<List<Encuesta>> ListarAsesores(string seccion)
        {
            if (string.IsNullOrWhiteSpace(seccion))
                throw new ArgumentException("La sección no puede estar vacía", nameof(seccion));

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_ASESORES}
                    @P_BLCK_CODE = {seccion}"
            ).ToListAsync();
        }

        public async Task<int> CrearAsignaturaEncuesta(Encuesta encuesta)
        {
            if (encuesta == null)
                throw new ArgumentNullException(nameof(encuesta));

            ValidarEncuesta(encuesta);

            var resultado = await _sqlContext.Database.SqlQuery<int>(
                $@"EXEC {StoredProcedureNames.SP_CREAR_ENCUESTA_ASIGNATURA}
                @NOMBRE_ENCUESTA = {encuesta.NombreEncuesta},
                @DESCRIPCION_ENCUESTA = {encuesta.DescripcionEncuesta},
                @TIPO_ENCUESTA_ID = {encuesta.TipoEncuestaId},
                @TIPO_ENCUESTADO_ID = {encuesta.TipoEncuestadoId},
                @TIPO_PROGRAMA = {encuesta.TipoPrograma},
                @SEDE = {encuesta.Sede},
                @PERIODO_ID = {encuesta.PeriodoId},
                @SECCION_ID = {encuesta.SeccionId},
                @MODULO = {encuesta.Modulo},
                @DOCENTE = {encuesta.Docente},
                @FECHA_INICIO = {encuesta.FechaInicio},
                @FECHA_FIN = {encuesta.FechaFin},
                @ACTIVO = {(encuesta.Activo ? 1 : 0)},
                @FECHA_ENVIO = {DateTime.Now},
                @USUARIO_ENVIO = {encuesta.UsuarioCreacion}"
            ).FirstOrDefaultAsync();

            return resultado;
        }

        public async Task<List<Encuesta>> ListarAlumnos(string seccion, string asignatura)
        {
            if (string.IsNullOrWhiteSpace(seccion) || string.IsNullOrWhiteSpace(asignatura))
                throw new ArgumentException("La sección y asignatura no pueden estar vacías");

            return await _bannerContext.Database.SqlQuery<Encuesta>(
                $@"EXEC {StoredProcedureNames.SP_LISTAR_CORREOS_ENCUESTA}
                    @p_asignatura = {asignatura},
                    @p_seccion = {seccion}"
            ).ToListAsync();
        }

        public async Task<int> CrearPlantillaEncuesta(Encuesta encuesta)
        {
            if (encuesta == null)
                throw new ArgumentNullException(nameof(encuesta));

            ValidarEncuesta(encuesta);

            var resultado = await _sqlContext.Database.SqlQuery<int>(
                $@"EXEC {StoredProcedureNames.SP_CREAR_ENCUESTA_PLANTILLA}
                    @NOMBRE_ENCUESTA = {encuesta.NombreEncuesta},
                    @DESCRIPCION_ENCUESTA = {encuesta.DescripcionEncuesta},
                    @TIPO_ENCUESTA_ID = {encuesta.TipoEncuestaId},
                    @FECHA_CREACION = {DateTime.Now},
                    @USUARIO_CREACION = {encuesta.UsuarioCreacion}"
            ).FirstOrDefaultAsync();

            return resultado;
        }

        public async Task EditarEncuestaPlantilla(Encuesta encuesta)
        {
            if (encuesta == null)
                throw new ArgumentNullException(nameof(encuesta));
            if (encuesta.EncuestaId < 1)
                throw new ArgumentException("El ID de encuesta es inválido");

            await _sqlContext.Database.ExecuteSqlInterpolatedAsync(
                $@"EXEC {StoredProcedureNames.SP_EDITAR_PLANTILLA_ENCUESTA}
                    @ENCUESTA_ID = {encuesta.EncuestaId},
                    @NOMBRE_ENCUESTA = {encuesta.NombreEncuesta},
                    @DESCRIPCION_ENCUESTA = {encuesta.DescripcionEncuesta},
                    @TIPO_ENCUESTA = {encuesta.TipoEncuestaId},
                    @USUARIO_MODIFICACION = {encuesta.UsuarioModificacion},
                    @FECHA_MODIFICACION = {DateTime.Now}"
            );
        }

        public async Task EliminarEncuesta(int id, string usuario)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que 0");
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuario));

            await _sqlContext.Database.ExecuteSqlInterpolatedAsync(
               $@"EXEC {StoredProcedureNames.SP_ELIMINAR_ENCUESTA_PLANTILLA}
                    @ENCUESTA_ID = {id},
                    @USUARIO_MODIFICACION = {usuario},
                    @FECHA_MODIFICACION = {DateTime.Now}"
            );
        }

        public async Task EliminarBloque(int id, string usuario)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que 0");
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuario));

            await _sqlContext.Database.ExecuteSqlInterpolatedAsync(
                $@"EXEC {StoredProcedureNames.SP_ELIMINAR_BLOQUE_PLANTILLA}
                    @BLOQUE_ID = {id},
                    @USUARIO_MODIFICACION = {usuario},
                    @FECHA_MODIFICACION = {DateTime.Now}"
            );
        }

        public async Task EliminarPregunta(int id, string usuario)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que 0");
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuario));

            await _sqlContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"EXEC {StoredProcedureNames.SP_ELIMINAR_PREGUNTA_PLANTILLA}
                        @PREGUNTA_ID = {id},
                        @USUARIO_MODIFICACION = {usuario},
                        @FECHA_MODIFICACION = {DateTime.Now}"
            );
        }

        private void ValidarEncuesta(Encuesta encuesta)
        {
            if (string.IsNullOrWhiteSpace(encuesta.NombreEncuesta))
                throw new ArgumentException("El nombre de la encuesta no puede estar vacío");
            if (string.IsNullOrWhiteSpace(encuesta.UsuarioCreacion))
                throw new ArgumentException("El usuario de creación no puede estar vacío");
            if (encuesta.TipoEncuestaId < 1)
                throw new ArgumentException("El tipo de encuesta es inválido");
        }

        public async Task InsertarEncuestasPorAsignaturaBulkAsync(int encuestaId, string usuario, List<string> alumnos, int tipoEncuestadoId)
        {
            if (encuestaId < 1)
                throw new ArgumentOutOfRangeException(nameof(encuestaId));
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío");
            if (alumnos == null || !alumnos.Any())
                throw new ArgumentException("La lista de alumnos no puede estar vacía");

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
    }
}