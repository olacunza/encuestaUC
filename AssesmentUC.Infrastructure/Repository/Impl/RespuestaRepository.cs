using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class RespuestaRepository : IRespuestaRepository
    {
        public readonly string _connectionStringBDPRACTICAS;
        public readonly string _connectionStringBANNER;

        public RespuestaRepository(IConfiguration configuration)
        {
            _connectionStringBDPRACTICAS = configuration.GetConnectionString("BDPRACTICAS")!;
            _connectionStringBANNER = configuration.GetConnectionString("BANNER")!;
        }

        public async Task<List<RespuestaEncuesta>> ListarEncuestasRespondidasRepository(string alumnoId)
        {
            var lista = new List<RespuestaEncuesta>();

            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            var cmd = new SqlCommand("ENCUESTA.sp_ListarRencuestasRespondidas", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ALUMNO_ID", alumnoId);

            using var reader = await cmd.ExecuteReaderAsync();
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

            return lista;
        }
        public async Task<Encuesta> ListaPreguntasEncuestaRepository(int encuestaId, string encuestadoDNI)
        {
            Encuesta encuesta = null!;
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.sp_ListarAsignaturaEncuestaId", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
                    cmd.Parameters.AddWithValue("@DNI_ENCUESTADO", encuestadoDNI);

                    using var reader = await cmd.ExecuteReaderAsync();

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
                        //throw new KeyNotFoundException($"No se encontró ninguna encuesta con el ID {encuestaId}");
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

                return encuesta;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string?> BuscarNombreDocente(string dniDocente)
        {
            using var connection = new OracleConnection(_connectionStringBANNER);
            using var command = new OracleCommand("P_NOMBRE_DOCENTE_DNI", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("P_BLCK_CODE", OracleDbType.Varchar2).Value = dniDocente;

            var cursor = command.Parameters.Add("O_CURSOR", OracleDbType.RefCursor);
            cursor.Direction = ParameterDirection.Output;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["DOCENTE"]?.ToString();
            }

            return "Sin nombre Docente";
        }


        public async Task<List<Encuesta>> ListaEncuestaAsignaturaPendienteRepository(string alumnoId)
        {
            var lista = new List<Encuesta>();

            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();

            var cmd = new SqlCommand("ENCUESTA.sp_ListarEncuestasPendientes", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ALUMNO_ID", alumnoId);

            using var reader = await cmd.ExecuteReaderAsync();
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

            return lista;
        }

        public async Task RegistrarRespuestaRepository(RespuestaEncuesta respuestaModel)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int respuestaEncuestaId;

                using (var cmd = new SqlCommand("ENCUESTA.sp_CrearRespuestaEncuesta", connection, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", respuestaModel.EncuestaId);
                    cmd.Parameters.AddWithValue("@ALUMNO_ID", respuestaModel.AlumnoId);
                    cmd.Parameters.AddWithValue("@FECHA_RESPUESTA", DateTime.Now);
                    cmd.Parameters.AddWithValue("@COMPLETADO", respuestaModel.Completado);

                    var result = await cmd.ExecuteScalarAsync();
                    respuestaEncuestaId = Convert.ToInt32(result);
                }

                foreach ( var r in respuestaModel.Respuestas)
                {
                    using var cmd = new SqlCommand("ENCUESTA.sp_CrearRespuestaPregunta", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RESPUESTA_ENCUESTA_ID", respuestaEncuestaId);
                    cmd.Parameters.AddWithValue("@ENCUESTA_PREGUNTA_ID", r.EncuestaPreguntaId);
                    cmd.Parameters.AddWithValue("@VALOR_RESPUESTA", (object?)r.ValorRespuesta ?? DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
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

        public async Task ActualizarEncuestaCompletadaRepository(int encuestaId, string alumnoId)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("ENCUESTA.sp_ActualizarEncuestaEnviada", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ENCUESTA_ASIGNATURA_ID", encuestaId);
                cmd.Parameters.AddWithValue("@ALUMNO_ID", alumnoId);

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

        public async Task<bool> VerificarSiRespondioRepository(int encuestaId, string alumnoId)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var cmd = new SqlCommand("ENCUESTA.sp_ValidarRespuestaAlumno", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
            cmd.Parameters.AddWithValue("@ALUMNO_ID", alumnoId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> VerificarEncuestaActivaRepository(int encuestaId)
        {
            using var connection = new SqlConnection(_connectionStringBDPRACTICAS);
            await connection.OpenAsync();
            using var cmd = new SqlCommand("ENCUESTA.sp_ValidarEncuestaActiva", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

    }
}
