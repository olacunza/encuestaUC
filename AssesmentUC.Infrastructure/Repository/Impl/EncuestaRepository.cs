using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class EncuestaRepository : IEncuestaRepository
    {
        private readonly string _connectionString;

        public EncuestaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Encuesta>> ListarEncuestasRepository()
        {
            var encuestas = new List<Encuesta>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.SSP_LISTAR_ENCUESTAS", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var encuesta = new Encuesta()
                            {
                                EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                                NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                                DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                                TipoPrograma = reader.GetInt32(reader.GetOrdinal("TIPO_PROGRAMA")),
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

        public async Task<Encuesta> ListarEncuestaIdRepository(int id)
        {
            Encuesta encuesta = null!;
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                using (var cmd = new SqlCommand("ENCUESTA.SSP_LISTAR_ENCUESTA_ID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", id);

                    using var reader = await cmd.ExecuteReaderAsync();
                    
                    if ( await reader.ReadAsync())
                    {
                        encuesta = new Encuesta
                        {
                            EncuestaId = reader.GetInt32(reader.GetOrdinal("ENCUESTA_ID")),
                            NombreEncuesta = reader.GetString(reader.GetOrdinal("NOMBRE_ENCUESTA")),
                            DescripcionEncuesta = reader.GetString(reader.GetOrdinal("DESCRIPCION_ENCUESTA")),
                            TipoPrograma = reader.GetInt32(reader.GetOrdinal("TIPO_PROGRAMA")),
                            Sede = reader.GetInt32(reader.GetOrdinal("SEDE")),
                            Periodo = reader.GetString(reader.GetOrdinal("PERIODO")),
                            Seccion = reader.GetString(reader.GetOrdinal("SECCION")),
                            FechaInicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")),
                            FechaFin = reader.GetDateTime(reader.GetOrdinal("FECHA_FIN")),
                            Completado = reader.GetBoolean(reader.GetOrdinal("COMPLETADO")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("ACTIVO")),
                            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FECHA_CREACION")),
                            Bloques = new List<EncuestaBloque>()
                        };
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

        public async Task<int> CrearEncuestaRepository(Encuesta encuesta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int encuestaId;
                using (var cmd = new SqlCommand("ENCUESTA.ISP_CREAR_ENCUESTA", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NOMBRE_ENCUESTA", encuesta.NombreEncuesta);
                    cmd.Parameters.AddWithValue("@DESCRIPCION_ENCUESTA", encuesta.DescripcionEncuesta);
                    cmd.Parameters.AddWithValue("@TIPO_PROGRAMA", encuesta.TipoPrograma);
                    cmd.Parameters.AddWithValue("@SEDE", encuesta.Sede);
                    cmd.Parameters.AddWithValue("@PERIODO", encuesta.Periodo);
                    cmd.Parameters.AddWithValue("@SECCION", encuesta.Seccion);
                    cmd.Parameters.AddWithValue("@FECHA_INICIO", encuesta.FechaInicio);
                    cmd.Parameters.AddWithValue("@FECHA_FIN", encuesta.FechaFin);
                    cmd.Parameters.AddWithValue("@COMPLETADO", encuesta.Completado);
                    cmd.Parameters.AddWithValue("@ACTIVO", encuesta.Activo);
                    cmd.Parameters.AddWithValue("@ESTADO", encuesta.Estado);
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

        public async Task EditarEncuestaRepository(Encuesta encuesta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using ( var cmd = new SqlCommand("ENCUESTA.USP_EDITAR_ENCUESTA", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuesta.EncuestaId);
                    cmd.Parameters.AddWithValue("@NOMBRE_ENCUESTA", (object?)encuesta.NombreEncuesta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DESCRIPCION_ENCUESTA", (object?)encuesta.DescripcionEncuesta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIPO_PROGRAMA", (object?)encuesta.TipoPrograma);
                    cmd.Parameters.AddWithValue("@SEDE", (object?)encuesta.Sede);
                    cmd.Parameters.AddWithValue("@PERIODO", (object?)encuesta.Periodo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SECCION", (object?)encuesta.Seccion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FECHA_INICIO", encuesta.FechaInicio);
                    cmd.Parameters.AddWithValue("@FECHA_FIN", encuesta.FechaFin);
                    cmd.Parameters.AddWithValue("@ACTIVO", encuesta.Activo);
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
            using var connection = new SqlConnection(_connectionString);
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
            using var connection = new SqlConnection(_connectionString);
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
            using var connection = new SqlConnection(_connectionString);
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
