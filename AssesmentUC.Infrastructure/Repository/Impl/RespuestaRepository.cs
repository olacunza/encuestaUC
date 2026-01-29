using AssesmentUC.Infrastructure.Data;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class RespuestaRepository : IRespuestaRepository
    {
        private readonly AppDbContext _sqlContext;
        private readonly BannerDbContext _bannerContext;

        public RespuestaRepository(AppDbContext sqlContext, BannerDbContext bannerContext)
        {
            _sqlContext = sqlContext;
            _bannerContext = bannerContext;
        }

        public async Task<List<RespuestaEncuesta>> ListarEncuestasRespondidasRepository(string alumnoId)
        {
            var lista = new List<RespuestaEncuesta>();
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ListarRencuestasRespondidas_UC";
                command.CommandType = CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@ALUMNO_ID";
                parameter.Value = alumnoId ?? string.Empty;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new RespuestaEncuesta
                    {
                        EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                        AlumnoId = alumnoId,
                        FechaRespuesta = reader.GetDateTime(reader.GetOrdinal("FECHA_RESPUESTA")),
                        Completado = reader.GetBoolean(reader.GetOrdinal("COMPLETADO"))
                    });
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return lista;
        }

        public async Task<Encuesta> ListaPreguntasEncuestaRepository(int encuestaId, string encuestadoDNI)
        {
            Encuesta encuesta = null;
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ListarAsignaturaEncuestaId_UC";
                command.CommandType = CommandType.StoredProcedure;

                var paramEncuestaId = command.CreateParameter();
                paramEncuestaId.ParameterName = "@ENCUESTA_ID";
                paramEncuestaId.Value = encuestaId;
                command.Parameters.Add(paramEncuestaId);

                var paramDni = command.CreateParameter();
                paramDni.ParameterName = "@DNI_ENCUESTADO";
                paramDni.Value = encuestadoDNI ?? string.Empty;
                command.Parameters.Add(paramDni);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    encuesta = new Encuesta
                    {
                        EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                        NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                        DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                        NombreTipoEncuesta = reader.GetString(reader.GetOrdinal("TIPO_ENCUESTA")),
                        TipoPrograma = reader.GetString(reader.GetOrdinal("TIPO_PROGRAMA")),
                        Sede = reader.GetString(reader.GetOrdinal("SEDE")),
                        Periodo = reader.GetString(reader.GetOrdinal("PERIODO")),
                        Seccion = reader.GetString(reader.GetOrdinal("SECCION")),
                        Modulo = reader.GetString(reader.GetOrdinal("MODULO")),
                        DocenteId = reader.GetString(reader.GetOrdinal("DOCENTE")),
                        FechaInicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")),
                        FechaFin = reader.GetDateTime(reader.GetOrdinal("FECHA_FIN")),
                        Bloques = new List<EncuestaBloque>()
                    };
                }

                if (encuesta == null)
                {
                    return null;
                }

                if (await reader.NextResultAsync())
                {
                    var bloquesTemp = new List<EncuestaBloque>();
                    while (await reader.ReadAsync())
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

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var bloqueId = reader.GetInt32(reader.GetOrdinal("BLOQUE_ID"));
                        var bloque = encuesta.Bloques.FirstOrDefault(b => b.BloqueId == bloqueId);
                        if (bloque != null)
                        {
                            bloque.Preguntas.Add(new EncuestaPregunta
                            {
                                EncuestaDetalleId = reader.GetInt32(reader.GetOrdinal("PREGUNTA_ID")),
                                TextoPregunta = reader.GetString(reader.GetOrdinal("TEXTO_PREGUNTA")),
                                TipoPregunta = reader.GetString(reader.GetOrdinal("TIPO_PREGUNTA")),
                                OrdenPregunta = reader.GetInt32(reader.GetOrdinal("ORDEN")),
                                OpcionesJson = reader.IsDBNull(reader.GetOrdinal("OPCIONES_JSON")) ? null : reader.GetString(reader.GetOrdinal("OPCIONES_JSON"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListaPreguntasEncuestaRepository: {ex.Message}");
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return encuesta;
        }

        public async Task<string?> BuscarNombreDocente(string dniDocente)
        {
            var connection = _bannerContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                // Manejo específico para Oracle con REF CURSOR
                if (connection is OracleConnection oracleConnection)
                {
                    using var command = new OracleCommand("BANINST1.SZKENCU.P_NOMBRE_DOCENTE_DNI", oracleConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("P_BLCK_CODE", OracleDbType.Varchar2).Value = dniDocente ?? string.Empty;
                    command.Parameters.Add("O_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return reader["DOCENTE"]?.ToString() ?? "Sin nombre Docente";
                    }
                }
                else
                {
                    // Fallback para otros proveedores
                    using var command = connection.CreateCommand();
                    command.CommandText = "BANINST1.SZKENCU.P_NOMBRE_DOCENTE_DNI";
                    command.CommandType = CommandType.StoredProcedure;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "P_BLCK_CODE";
                    parameter.Value = dniDocente ?? string.Empty;
                    command.Parameters.Add(parameter);

                    using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return reader["DOCENTE"]?.ToString() ?? "Sin nombre Docente";
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return "Sin nombre Docente";
        }

        public async Task<List<Encuesta>> ListaEncuestaAsignaturaPendienteRepository(string alumnoId)
        {
            var lista = new List<Encuesta>();
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ListarEncuestasPendientes_UC";
                command.CommandType = CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@ALUMNO_ID";
                parameter.Value = alumnoId ?? string.Empty;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var item = new Encuesta
                    {
                        EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                        NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                        DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                        NombreTipoEncuesta = reader.GetString(reader.GetOrdinal("TIPO_ENCUESTA")),
                        FechaInicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")),
                        FechaFin = reader.GetDateTime(reader.GetOrdinal("FECHA_FIN"))
                    };

                    lista.Add(item);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return lista;
        }

        public async Task RegistrarRespuestaRepository(RespuestaEncuesta respuestaModel)
        {
            var connection = _sqlContext.Database.GetDbConnection();
            IDbContextTransaction transaction = null;

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                transaction = await _sqlContext.Database.BeginTransactionAsync();
                int respuestaEncuestaId;

                using var command1 = connection.CreateCommand();
                command1.CommandText = "ENC.sp_CrearRespuestaEncuesta_UC";
                command1.CommandType = CommandType.StoredProcedure;
                command1.Transaction = transaction.GetDbTransaction();

                var param1 = command1.CreateParameter();
                param1.ParameterName = "@ENCUESTA_ID";
                param1.Value = respuestaModel.EncuestaId;
                command1.Parameters.Add(param1);

                var param2 = command1.CreateParameter();
                param2.ParameterName = "@ALUMNO_ID";
                param2.Value = respuestaModel.AlumnoId ?? string.Empty;
                command1.Parameters.Add(param2);

                var param3 = command1.CreateParameter();
                param3.ParameterName = "@FECHA_RESPUESTA";
                param3.Value = DateTime.Now;
                command1.Parameters.Add(param3);

                var param4 = command1.CreateParameter();
                param4.ParameterName = "@COMPLETADO";
                param4.Value = respuestaModel.Completado;
                command1.Parameters.Add(param4);

                var result = await command1.ExecuteScalarAsync();
                respuestaEncuestaId = Convert.ToInt32(result);

                foreach (var r in respuestaModel.Respuestas)
                {
                    using var command2 = connection.CreateCommand();
                    command2.CommandText = "ENC.sp_CrearRespuestaPregunta_UC";
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.Transaction = transaction.GetDbTransaction();

                    var paramRespId = command2.CreateParameter();
                    paramRespId.ParameterName = "@RESPUESTA_ENCUESTA_ID";
                    paramRespId.Value = respuestaEncuestaId;
                    command2.Parameters.Add(paramRespId);

                    var paramPregId = command2.CreateParameter();
                    paramPregId.ParameterName = "@ENCUESTA_PREGUNTA_ID";
                    paramPregId.Value = r.EncuestaPreguntaId;
                    command2.Parameters.Add(paramPregId);

                    var paramValor = command2.CreateParameter();
                    paramValor.ParameterName = "@VALOR_RESPUESTA";
                    paramValor.Value = r.ValorRespuesta ?? (object)DBNull.Value;
                    command2.Parameters.Add(paramValor);

                    await command2.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();

                Console.WriteLine($"Error en RegistrarRespuestaRepository: {ex.Message}");
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public async Task ActualizarEncuestaCompletadaRepository(int encuestaId, string alumnoId)
        {
            var connection = _sqlContext.Database.GetDbConnection();
            IDbContextTransaction transaction = null;

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                transaction = await _sqlContext.Database.BeginTransactionAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ActualizarEncuestaEnviada_UC";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction.GetDbTransaction();

                var param1 = command.CreateParameter();
                param1.ParameterName = "@ENCUESTA_ASIGNATURA_ID";
                param1.Value = encuestaId;
                command.Parameters.Add(param1);

                var param2 = command.CreateParameter();
                param2.ParameterName = "@ALUMNO_ID";
                param2.Value = alumnoId ?? string.Empty;
                command.Parameters.Add(param2);

                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();

                Console.WriteLine($"Error en ActualizarEncuestaCompletadaRepository: {ex.Message}");
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public async Task<bool> VerificarSiRespondioRepository(int encuestaId, string alumnoId)
        {
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ValidarRespuestaAlumno_UC";
                command.CommandType = CommandType.StoredProcedure;

                var param1 = command.CreateParameter();
                param1.ParameterName = "@ENCUESTA_ID";
                param1.Value = encuestaId;
                command.Parameters.Add(param1);

                var param2 = command.CreateParameter();
                param2.ParameterName = "@ALUMNO_ID";
                param2.Value = alumnoId ?? string.Empty;
                command.Parameters.Add(param2);

                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) > 0;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public async Task<bool> VerificarEncuestaActivaRepository(int encuestaId)
        {
            var connection = _sqlContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "ENC.sp_ValidarEncuestaActiva_UC";
                command.CommandType = CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@ENCUESTA_ID";
                parameter.Value = encuestaId;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToBoolean(result);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }
    }
}