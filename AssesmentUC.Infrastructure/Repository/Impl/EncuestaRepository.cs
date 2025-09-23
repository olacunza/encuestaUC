using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Collections.Specialized.BitVector32;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class EncuestaRepository : IEncuestaRepository
    {
        private readonly string _connectionStringBDPRACTICAS;
        private readonly string _connectionStringBANNER;

        public EncuestaRepository(IConfiguration configuration)
        {
            _connectionStringBDPRACTICAS = configuration.GetConnectionString("BDPRACTICAS")!;
            _connectionStringBANNER = configuration.GetConnectionString("BANNER")!;
        }

        public async Task<List<Encuesta>> ListarPlantillaEncuestasRepository(int pageNumber, int pageSize)
        {
            var encuestas = new List<Encuesta>();
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.SSP_LISTAR_PLANTILLA_ENCUESTAS", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var encuesta = new Encuesta()
                            {
                                EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                                NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                                DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                                NombreTipoEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_TIPO_ENCUESTA")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FECHA_CREACION"))
                            };

                            encuestas.Add(encuesta);
                        }
                    }
                }
                return encuestas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Encuesta> ListarPlantillaEncuestaIdRepository(int id)
        {
            Encuesta encuesta = null!;
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.SSP_LISTAR_PLANTILLA_ENCUESTA_ID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", id);

                    using var reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        encuesta = new Encuesta
                        {
                            EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                            NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                            DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                            TipoEncuestaId = reader.GetInt32(reader.GetOrdinal("TIPO_ENCUESTA_ID")),
                            NombreTipoEncuesta = reader.GetString(reader.GetOrdinal("TIPO_ENCUESTA")),                          
                            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FECHA_CREACION")),
                            Bloques = new List<EncuestaBloque>()
                        };
                    }

                    if (encuesta == null)
                    {
                        throw new KeyNotFoundException($"No se encontró ninguna encuesta con el ID {id}");
                    }

                    if (await reader.NextResultAsync())
                    {
                        var bloquesTemp = new List<EncuestaBloque>();
                        while ( await reader.ReadAsync())
                        {
                            bloquesTemp.Add(new EncuestaBloque
                            {
                                BloqueId = reader.GetInt32(reader.GetOrdinal("BLOQUE_ID")),
                                TituloBloque = reader.GetString(reader.GetOrdinal("TITULO_BLOQUE")),
                                OrdenBloque = reader.GetInt32(reader.GetOrdinal("ORDEN")),
                                Preguntas = new List<EncuestaPregunta>()
                            });
                        }

                        encuesta.Bloques = bloquesTemp;

                    }

                    if ( await reader.NextResultAsync() )
                    {
                        while ( await reader.ReadAsync())
                        {
                            var bloqueId = reader.GetInt32(reader.GetOrdinal("BLOQUE_ID"));
                            var bloque = encuesta.Bloques.FirstOrDefault(b => b.BloqueId == bloqueId);
                            if ( bloque != null )
                            {
                                bloque.Preguntas.Add(new EncuestaPregunta
                                {
                                    EncuestaDetalleId = reader.GetInt32(reader.GetOrdinal("PREGUNTA_ID")),
                                    TextoPregunta = reader.GetString(reader.GetOrdinal("TEXTO_PREGUNTA")),
                                    TipoPregunta = reader.GetString(reader.GetOrdinal("TIPO_PREGUNTA")),
                                    OrdenPregunta = reader.GetInt32(reader.GetOrdinal("ORDEN")),
                                    OpcionesJson = reader.IsDBNull(reader.GetOrdinal("OPCIONES_JSON"))? null: reader.GetString(reader.GetOrdinal("OPCIONES_JSON"))
                                });
                            }
                        }
                    }
                }

                return encuesta;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }   

        public async Task<List<Encuesta>> ListarTipoEncuestaRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.SSP_LISTAR_TIPO_ENCUESTA", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            TipoEncuestaId = reader.GetInt32(reader.GetOrdinal("TIPO_ENCUESTA_ID")),
                            NombreTipoEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<Encuesta>> ListarSedesRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new OracleCommand("SSP_LISTAR_SEDES", connection))
                {
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            SedeId = reader.GetString(reader.GetOrdinal("SEDE_ID")),
                            Sede = reader.GetString(reader.GetOrdinal("NOMBRE_SEDE"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<Encuesta>> ListarPeriodosRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new OracleCommand("SSP_LISTAR_PERIODOS", connection))
                {
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            PeriodoId = reader.GetString(reader.GetOrdinal("PERIODO_ID")),
                            Periodo = reader.GetString(reader.GetOrdinal("NOMBRE_PERIODO"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        
        public async Task<List<Encuesta>> ListarSeccionesRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new OracleCommand("SSP_LISTAR_SECCIONES", connection))
                {
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            SeccionId = reader.GetString(reader.GetOrdinal("SECCION_ID")),
                            Seccion = reader.GetString(reader.GetOrdinal("NOMBRE_SECCION"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<Encuesta>> ListarAsignaturasRepository(string seccion)
        {
            var encuestas = new List<Encuesta>();
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new OracleCommand("SSP_LISTAR_ASIGNATURAS", connection))
                {
                    cmd.Parameters.Add("p_blck_code", OracleDbType.Varchar2, seccion, ParameterDirection.Input);
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            NRC = reader.GetString(reader.GetOrdinal("NRC")),
                            NombreAsignatura= reader.GetString(reader.GetOrdinal("NOMBRE_ASIGNATURA"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<Encuesta>> ListarTipoProgramaRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                
                using (var cmd = new OracleCommand("SSP_LISTAR_TIPO_PROGRAMA", connection))
                {
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var encuesta = new Encuesta
                        {
                            TipoProgramaId = reader.GetString(reader.GetOrdinal("TIPO_PROGRAMA_ID")),
                            TipoPrograma = reader.GetString(reader.GetOrdinal("NOMBRE_PROGRAMA"))
                        };

                        encuestas.Add(encuesta);
                    }
                }

                return encuestas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Encuesta> ListarDatosAsignaturaRepository(string asignatura)
        {
            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using var cmd = new OracleCommand("SSP_LISTAR_DATOS_ASIGNATURA", connection);
                cmd.Parameters.Add("p_blck_code", OracleDbType.Varchar2, asignatura, ParameterDirection.Input);
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.CommandType = CommandType.StoredProcedure;

                using var reader = await cmd.ExecuteReaderAsync();

                await reader.ReadAsync();

                return new Encuesta
                {
                    Modulo = reader.GetString(reader.GetOrdinal("MODULO")),
                    Docente = reader.GetString(reader.GetOrdinal("DOCENTE"))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> CrearAsignaturaEncuestaRepository(Encuesta encuesta)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int encuestaId;
                using (var cmd = new SqlCommand("ENCUESTA.ISP_CREAR_ENCUESTA_ASIGNATURA", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NOMBRE_ENCUESTA", encuesta.NombreEncuesta);
                    cmd.Parameters.AddWithValue("@DESCRIPCION_ENCUESTA", encuesta.DescripcionEncuesta);
                    cmd.Parameters.AddWithValue("@TIPO_ENCUESTA_ID", encuesta.TipoEncuestaId);
                    cmd.Parameters.AddWithValue("@TIPO_PROGRAMA_ID", encuesta.TipoProgramaId);
                    cmd.Parameters.AddWithValue("@SEDE_ID", encuesta.SedeId);
                    cmd.Parameters.AddWithValue("@PERIODO_ID", encuesta.PeriodoId);
                    cmd.Parameters.AddWithValue("@SECCION_ID", encuesta.SeccionId);
                    cmd.Parameters.AddWithValue("@ASIGNATURA", encuesta.NombreAsignatura);      //VALIDAR SI ES NOMBREASIGNATURA O NRC
                    cmd.Parameters.AddWithValue("@MODULO", encuesta.Modulo);
                    cmd.Parameters.AddWithValue("@DOCENTE", encuesta.Docente);
                    cmd.Parameters.AddWithValue("@FECHA_INICIO", encuesta.FechaInicio);
                    cmd.Parameters.AddWithValue("@FECHA_FIN", encuesta.FechaFin);
                    cmd.Parameters.AddWithValue("@ACTIVO", encuesta.Activo);
                    cmd.Parameters.AddWithValue("@FECHA_ENVIO", DateTime.Now);
                    cmd.Parameters.AddWithValue("@USUARIO_ENVIO", encuesta.UsuarioCreacion);

                    var result = await cmd.ExecuteScalarAsync();
                    if (result == null || result == DBNull.Value)
                        throw new InvalidOperationException("El SP devolvió valor NULL");
                    
                    encuestaId = Convert.ToInt32(result);
                }

                if ( encuesta.Bloques != null && encuesta.Bloques.Count > 0 )
                {
                    foreach (var bloque in encuesta.Bloques)
                    {
                        int bloqueId;
                        using (var cmdBloque = new SqlCommand("ENCUESTA.ISP_CREAR_BLOQUE_ASIGNATURA", connection, transaction))
                        {
                            cmdBloque.CommandType = CommandType.StoredProcedure;
                            cmdBloque.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
                            cmdBloque.Parameters.AddWithValue("@TITULO_BLOQUE", bloque.TituloBloque);
                            cmdBloque.Parameters.AddWithValue("@ORDEN", bloque.OrdenBloque);
                            cmdBloque.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                            cmdBloque.Parameters.AddWithValue("@USUARIO_CREACION", encuesta.UsuarioCreacion);

                            var resultBloque = await cmdBloque.ExecuteScalarAsync();
                            if (resultBloque == null || resultBloque == DBNull.Value)
                                throw new InvalidOperationException("El SP del bloque devolvió NULL");

                            bloqueId = Convert.ToInt32(resultBloque);
                        }

                        if ( bloque.Preguntas != null && bloque.Preguntas.Count > 0)
                        {
                            foreach (var pregunta in bloque.Preguntas)
                            {
                                using (var cmdPregunta = new SqlCommand("ENCUESTA.ISP_CREAR_PREGUNTA_ASIGNATURA", connection, transaction))
                                {
                                    cmdPregunta.CommandType = CommandType.StoredProcedure;
                                    cmdPregunta.Parameters.AddWithValue("@BLOQUE_ID", bloqueId);
                                    cmdPregunta.Parameters.AddWithValue("@TEXTO_PREGUNTA", pregunta.TextoPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@TIPO_PREGUNTA", pregunta.TipoPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@ORDEN", pregunta.OrdenPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@OPCIONES_JSON", (object?)pregunta.OpcionesJson ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                                    cmdPregunta.Parameters.AddWithValue("@USUARIO_CREACION", pregunta.UsuarioCreacion);

                                    await cmdPregunta.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                transaction.Commit();
                return encuestaId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task<List<AlumnoCorreo>> ListarCorreosEncuestaAsignaturaRepository(string asignatura)
        {
            var lista = new List<AlumnoCorreo>();

            using var connection = new OracleConnection(_connectionStringBANNER);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new OracleCommand("SSP_LISTAR_CORREOS_ENCUESTA", connection))
                {
                    cmd.Parameters.Add("p_blck_code", OracleDbType.Varchar2, asignatura, ParameterDirection.Input);
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var alumno = new AlumnoCorreo
                        {
                            AlumnoId = reader.GetString(reader.GetOrdinal("ALUMNO_ID")),
                            CorreoAlumno = reader.GetString(reader.GetOrdinal("CORREO_USUARIO"))
                        };

                        lista.Add(alumno);
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task InsertarEncuestasPorAsignaturaBulkAsync(int encuestaId, string usuario, List<string> alumnos)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            var table = new DataTable();
            table.Columns.Add("ENCUESTA_ASIGNATURA_ID", typeof(int));
            table.Columns.Add("ALUMNO_ID", typeof(string));
            table.Columns.Add("COMPLETADO", typeof(bool));
            table.Columns.Add("FECHA_CREACION", typeof(DateTime));
            table.Columns.Add("USUARIO_CREACION", typeof(string));

            foreach (var alumnoId in alumnos)
            {
                table.Rows.Add(encuestaId, alumnoId, false, DateTime.Now, usuario);
            }

            using var bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = "ENCUESTA.ENCUESTA_ASIGNATURA_ALUMNO"
            };

            bulk.ColumnMappings.Add("ENCUESTA_ASIGNATURA_ID", "ENCUESTA_ASIGNATURA_ID");
            bulk.ColumnMappings.Add("ALUMNO_ID", "ALUMNO_ID");
            bulk.ColumnMappings.Add("COMPLETADO", "COMPLETADO");
            bulk.ColumnMappings.Add("FECHA_CREACION", "FECHA_CREACION");
            bulk.ColumnMappings.Add("USUARIO_CREACION", "USUARIO_CREACION");

            await bulk.WriteToServerAsync(table);
        }



        public async Task<int> CrearPlantillaEncuestaRepository(Encuesta encuesta)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int encuestaId;
                using (var cmd = new SqlCommand("ENCUESTA.ISP_CREAR_ENCUESTA_PLANTILLA", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NOMBRE_ENCUESTA", encuesta.NombreEncuesta);
                    cmd.Parameters.AddWithValue("@DESCRIPCION_ENCUESTA", encuesta.DescripcionEncuesta);
                    cmd.Parameters.AddWithValue("@TIPO_ENCUESTA_ID", encuesta.TipoEncuestaId);
                    cmd.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                    cmd.Parameters.AddWithValue("@USUARIO_CREACION", encuesta.UsuarioCreacion);

                    var result = await cmd.ExecuteScalarAsync();
                    if (result == null || result == DBNull.Value)
                        throw new InvalidOperationException("El SP devolvió valor NULL");
                    
                    encuestaId = Convert.ToInt32(result);
                }

                if ( encuesta.Bloques != null && encuesta.Bloques.Count > 0 )
                {
                    foreach (var bloque in encuesta.Bloques)
                    {
                        int bloqueId;
                        using (var cmdBloque = new SqlCommand("ENCUESTA.ISP_CREAR_BLOQUE", connection, transaction))
                        {
                            cmdBloque.CommandType = CommandType.StoredProcedure;
                            cmdBloque.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
                            cmdBloque.Parameters.AddWithValue("@TITULO_BLOQUE", bloque.TituloBloque);
                            cmdBloque.Parameters.AddWithValue("@ORDEN", bloque.OrdenBloque);
                            cmdBloque.Parameters.AddWithValue("@ESTADO", bloque.Estado);
                            cmdBloque.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                            cmdBloque.Parameters.AddWithValue("@USUARIO_CREACION", encuesta.UsuarioCreacion);

                            var resultBloque = await cmdBloque.ExecuteScalarAsync();
                            if (resultBloque == null || resultBloque == DBNull.Value)
                                throw new InvalidOperationException("El SP del bloque devolvió NULL");

                            bloqueId = Convert.ToInt32(resultBloque);
                        }

                        if ( bloque.Preguntas != null && bloque.Preguntas.Count > 0)
                        {
                            foreach (var pregunta in bloque.Preguntas)
                            {
                                using (var cmdPregunta = new SqlCommand("ENCUESTA.ISP_CREAR_PREGUNTA", connection, transaction))
                                {
                                    cmdPregunta.CommandType = CommandType.StoredProcedure;
                                    cmdPregunta.Parameters.AddWithValue("@BLOQUE_ID", bloqueId);
                                    cmdPregunta.Parameters.AddWithValue("@TEXTO_PREGUNTA", pregunta.TextoPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@TIPO_PREGUNTA", pregunta.TipoPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@ORDEN", pregunta.OrdenPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@OPCIONES_JSON", (object?)pregunta.OpcionesJson ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@ESTADO", pregunta.Estado);
                                    cmdPregunta.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                                    cmdPregunta.Parameters.AddWithValue("@USUARIO_CREACION", pregunta.UsuarioCreacion);

                                    await cmdPregunta.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                transaction.Commit();
                return encuestaId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task EditarEncuestaPlantillaRepository(Encuesta encuesta)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using ( var cmd = new SqlCommand("ENCUESTA.USP_EDITAR_PLANTILLA_ENCUESTA", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuesta.EncuestaId);
                    cmd.Parameters.AddWithValue("@NOMBRE_ENCUESTA", (object?)encuesta.NombreEncuesta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DESCRIPCION_ENCUESTA", (object?)encuesta.DescripcionEncuesta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIPO_ENCUESTA", (object?)encuesta.TipoEncuestaId);                    
                    cmd.Parameters.AddWithValue("@USUARIO_MODIFICACION", encuesta.UsuarioModificacion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                    await cmd.ExecuteNonQueryAsync();

                }

                if ( encuesta.Bloques != null && encuesta.Bloques.Count > 0 )
                {
                    foreach ( var bloque in encuesta.Bloques)
                    {
                        int bloqueId = bloque.BloqueId;

                        if ( bloqueId > 0 )
                        {
                            using var cmdBloque = new SqlCommand("ENCUESTA.USP_EDITAR_BLOQUE", connection, transaction);
                            cmdBloque.CommandType = CommandType.StoredProcedure;
                            cmdBloque.Parameters.AddWithValue("@BLOQUE_ID", bloqueId);
                            cmdBloque.Parameters.AddWithValue("@TITULO_BLOQUE", (object?)bloque.TituloBloque ?? DBNull.Value);
                            cmdBloque.Parameters.AddWithValue("@ORDEN", bloque.OrdenBloque);
                            cmdBloque.Parameters.AddWithValue("@USUARIO_MODIFICACION", encuesta.UsuarioModificacion ?? (object)DBNull.Value);
                            cmdBloque.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                            await cmdBloque.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            using var cmdBloque = new SqlCommand("ENCUESTA.ISP_CREAR_BLOQUE", connection, transaction);
                            cmdBloque.CommandType = CommandType.StoredProcedure;
                            cmdBloque.Parameters.AddWithValue("@ENCUESTA_ID", encuesta.EncuestaId);
                            cmdBloque.Parameters.AddWithValue("@TITULO_BLOQUE", (object?)bloque.TituloBloque ?? DBNull.Value);
                            cmdBloque.Parameters.AddWithValue("@ORDEN", bloque.OrdenBloque);
                            cmdBloque.Parameters.AddWithValue("@ESTADO", bloque.Estado);
                            cmdBloque.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                            cmdBloque.Parameters.AddWithValue("@USUARIO_CREACION", encuesta.UsuarioModificacion ?? (object)DBNull.Value);

                            var result = await cmdBloque.ExecuteScalarAsync();
                            bloqueId = Convert.ToInt32(result);
                        }

                        if ( bloque.Preguntas != null && bloque.Preguntas.Count > 0 )
                        {
                            foreach ( var pregunta in bloque.Preguntas)
                            {
                                if ( pregunta.EncuestaDetalleId > 0)
                                {
                                    using var cmdPregunta = new SqlCommand("ENCUESTA.USP_EDITAR_PREGUNTA", connection, transaction);
                                    cmdPregunta.CommandType = CommandType.StoredProcedure;
                                    cmdPregunta.Parameters.AddWithValue("@PREGUNTA_ID", pregunta.EncuestaDetalleId);
                                    cmdPregunta.Parameters.AddWithValue("@TEXTO_PREGUNTA", (object?)pregunta.TextoPregunta ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@TIPO_PREGUNTA", (object?)pregunta.TipoPregunta ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@ORDEN", pregunta.OrdenPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@OPCIONES_JSON", (object?)pregunta.OpcionesJson ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@USUARIO_MODIFICACION", encuesta.UsuarioModificacion ?? (object)DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                                    await cmdPregunta.ExecuteNonQueryAsync();
                                }
                                else
                                {
                                    using var cmdPregunta = new SqlCommand("ENCUESTA.ISP_CREAR_PREGUNTA", connection, transaction);
                                    cmdPregunta.CommandType = CommandType.StoredProcedure;
                                    cmdPregunta.Parameters.AddWithValue("@BLOQUE_ID", bloqueId);
                                    cmdPregunta.Parameters.AddWithValue("@TEXTO_PREGUNTA", (object?)pregunta.TextoPregunta ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@TIPO_PREGUNTA", (object?)pregunta.TipoPregunta ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@ORDEN", pregunta.OrdenPregunta);
                                    cmdPregunta.Parameters.AddWithValue("@OPCIONES_JSON", (object?)pregunta.OpcionesJson ?? DBNull.Value);
                                    cmdPregunta.Parameters.AddWithValue("@ESTADO", pregunta.Estado);
                                    cmdPregunta.Parameters.AddWithValue("@FECHA_CREACION", DateTime.Now);
                                    cmdPregunta.Parameters.AddWithValue("@USUARIO_CREACION", encuesta.UsuarioModificacion ?? (object)DBNull.Value);

                                    await cmdPregunta.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task EliminarEncuestaRepository(int id, string usuario)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("ENCUESTA.USP_ELIMINAR_ENCUESTA", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ENCUESTA_ID", id);
                cmd.Parameters.AddWithValue("@USUARIO_MODIFICACION", usuario);
                cmd.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task EliminarBloqueRepository(int id, string usuario)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("ENCUESTA.USP_ELIMINAR_BLOQUE", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BLOQUE_ID", id);
                cmd.Parameters.AddWithValue("@USUARIO_MODIFICACION", usuario);
                cmd.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task EliminarPreguntaRepository(int id, string usuario)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("ENCUESTA.USP_ELIMINAR_PREGUNTA", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PREGUNTA_ID", id);
                cmd.Parameters.AddWithValue("@USUARIO_MODIFICACION", usuario);
                cmd.Parameters.AddWithValue("@FECHA_MODIFICACION", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
