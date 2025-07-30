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

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class RespuestaRepository : IRespuestaRepository
    {
        public readonly string _connectionString;

        public RespuestaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> ListarEncuestaRespuestaRepository()
        {
            var rtpaRepository = _connectionString;
            return rtpaRepository;
        }

        public async Task RegistrarRespuestaRepository(RespuestaEncuesta respuestaModel)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int respuestaEncuestaId;

                using (var cmd = new SqlCommand("ENCUESTA.ISP_CREAR_RESPUESTA_ENCUESTA", connection, transaction))
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
                    using var cmd = new SqlCommand("ENCUESTA.ISP_CREAR_RESPUESTA_PREGUNTA", connection, transaction);
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
        public async Task<bool> VerificarSiRespondioRepository(int encuestaId, string alumnoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var cmd = new SqlCommand("ENCUESTA.VALIDAR_RESPUESTA_ALUMNO", connection);
            cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
            cmd.Parameters.AddWithValue("@ALUMNO_ID", alumnoId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> VerificarEncuestaActivaRepository(int encuestaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var cmd = new SqlCommand("ENCUESTA.VALIDAR_ENCUESTA_ACTIVA", connection);
            cmd.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

    }
}
