using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static System.Collections.Specialized.BitVector32;
using static AssesmentUC.Model.Entity.ReporteEncuestas;

namespace AssesmentUC.Infrastructure.Repository.Impl
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly string _connectionStringBDPRACTICAS;

        public ReporteRepository(IConfiguration configuration)
        {
            _connectionStringBDPRACTICAS = configuration.GetConnectionString("BDPRACTICAS")!;
        }

        public async Task<ReporteEncuesta> ExportarValoresEncuestaAlumno(int encuestaId)
        {
            var rows = new List<ReportePlano>();
            var resumen = new ReporteResumen();

            using (var connection = new SqlConnection(_connectionStringBDPRACTICAS))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ENCUESTA.sp_ListarReporteEncuestaAlumno", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new ReportePlano
                            {
                                BLOQUE_ID = reader["BLOQUE_ID"] != DBNull.Value ? Convert.ToInt32(reader["BLOQUE_ID"]) : 0,
                                TITULO_BLOQUE = reader["TITULO_BLOQUE"]?.ToString() ?? "(Sin bloque)",
                                TEXTO_PREGUNTA = reader["TEXTO_PREGUNTA"]?.ToString() ?? string.Empty,
                                PROMEDIO_SCORE = reader["PROMEDIO_SCORE"] != DBNull.Value ? Convert.ToDecimal(reader["PROMEDIO_SCORE"]) : (decimal?)null,
                                PCT_1 = reader["PCT_1"] != DBNull.Value ? Convert.ToDecimal(reader["PCT_1"]) : 0,
                                PCT_2 = reader["PCT_2"] != DBNull.Value ? Convert.ToDecimal(reader["PCT_2"]) : 0,
                                PCT_3 = reader["PCT_3"] != DBNull.Value ? Convert.ToDecimal(reader["PCT_3"]) : 0,
                                PCT_4 = reader["PCT_4"] != DBNull.Value ? Convert.ToDecimal(reader["PCT_4"]) : 0,
                                PCT_5 = reader["PCT_5"] != DBNull.Value ? Convert.ToDecimal(reader["PCT_5"]) : 0
                            });
                        }

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            resumen = new ReporteResumen
                            {
                                CantidadMatriculados = reader["CANTIDAD_MATRICULADOS"] != DBNull.Value ? Convert.ToInt32(reader["CANTIDAD_MATRICULADOS"]) : 0,
                                CantidadEncuestados = reader["CANTIDAD_ENCUESTADOS"] != DBNull.Value ? Convert.ToInt32(reader["CANTIDAD_ENCUESTADOS"]) : 0,
                                PorcentajeEncuestados = reader["PORCENTAJE_ENCUESTADOS"] != DBNull.Value ? Convert.ToDecimal(reader["PORCENTAJE_ENCUESTADOS"]) : 0
                            };
                        }
                    }
                }
            }

            var bloques = rows
                .GroupBy(r => r.TITULO_BLOQUE)
                .Select(g => new ReporteBloque
                {
                    BloqueId = g.First().BLOQUE_ID,
                    TituloBloque = g.Key,
                    Preguntas = g.Select(p => new ReportePregunta
                    {
                        TextoPregunta = p.TEXTO_PREGUNTA,
                        Promedio = p.PROMEDIO_SCORE ?? 0,
                        PCT1 = p.PCT_1,
                        PCT2 = p.PCT_2,
                        PCT3 = p.PCT_3,
                        PCT4 = p.PCT_4,
                        PCT5 = p.PCT_5
                    }).ToList(),
                    PromedioBloque = Math.Round(
                        g.Where(p => p.PROMEDIO_SCORE.HasValue)
                         .Select(p => p.PROMEDIO_SCORE!.Value)
                         .DefaultIfEmpty(0)
                         .Average(), 2)
                })
                .ToList();

            var promedioTotal = Math.Round(
                bloques.Select(b => b.PromedioBloque ?? 0).DefaultIfEmpty(0).Average(), 2);

            resumen.PromedioTotalScore = promedioTotal;

            return new ReporteEncuesta
            {
                EncuestaId = encuestaId,
                TipoEncuestado = "Alumno",
                Resumen = resumen,
                Bloques = bloques
            };
        }

        public async Task<ReporteEncuesta> ExportarValoresEncuestaDocente(int encuestaId)
        {
            var rows = new List<ReportePlano>();
            var cabeceraReporte = new ReporteEncuesta();
            decimal promedioTotal = 0;

            using (var connection = new SqlConnection(_connectionStringBDPRACTICAS))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ENCUESTA.sp_ListarReporteEncuestaDocente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new ReportePlano
                            {
                                TITULO_BLOQUE = reader["TITULO_BLOQUE"]?.ToString() ?? "(Sin bloque)",
                                TEXTO_PREGUNTA = reader["TEXTO_PREGUNTA"]?.ToString() ?? string.Empty,
                                PROMEDIO_SCORE = reader["PROMEDIO_SCORE"] != DBNull.Value ? Convert.ToDecimal(reader["PROMEDIO_SCORE"]) : (decimal?)null
                            });
                        }

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            promedioTotal = reader["PROMEDIO_TOTAL_SCORE"] != DBNull.Value
                                ? Convert.ToDecimal(reader["PROMEDIO_TOTAL_SCORE"])
                                : 0;
                        }

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            cabeceraReporte.NombreEncuesta = reader["NOMBRE_ENCUESTA"].ToString() ?? "Sin nombre";
                            cabeceraReporte.Docente = reader["DOCENTE"].ToString() ?? "Sin Docente";
                            cabeceraReporte.Periodo = reader["PERIODO"].ToString() ?? "Sin Periodo";
                            cabeceraReporte.Seccion = reader["SECCION"].ToString() ?? "Sin Seccion";
                            cabeceraReporte.FechaInicio = (DateTime)reader["FECHA_INICIO"];
                            cabeceraReporte.FechaFin = (DateTime)reader["FECHA_FIN"];
                        }
                    }
                }
            }

            var bloques = rows
                .GroupBy(r => r.TITULO_BLOQUE)
                .Select(g => new ReporteBloque
                {
                    TituloBloque = g.Key,
                    Preguntas = g.Select(p => new ReportePregunta
                    {
                        TextoPregunta = p.TEXTO_PREGUNTA,
                        Promedio = p.PROMEDIO_SCORE ?? 0
                    }).ToList(),
                    PromedioBloque = Math.Round(
                        g.Where(p => p.PROMEDIO_SCORE.HasValue)
                         .Select(p => p.PROMEDIO_SCORE!.Value)
                         .DefaultIfEmpty(0)
                         .Average(), 2)
                })
                .ToList();

            return new ReporteEncuesta
            {
                EncuestaId = encuestaId,
                TipoEncuestado = "Docente",
                NombreEncuesta = cabeceraReporte.NombreEncuesta,
                Docente = cabeceraReporte.Docente,
                Periodo = cabeceraReporte.Periodo,
                Seccion = cabeceraReporte.Seccion,
                FechaInicio = cabeceraReporte.FechaInicio,
                FechaFin = cabeceraReporte.FechaFin,
                Resumen = new ReporteResumen { PromedioTotalScore = promedioTotal },
                Bloques = bloques
            };
        }

        public async Task<ReporteEncuesta> ExportarValoresEncuestaAsesor(int encuestaId)
        {
            var rows = new List<ReportePlano>();
            decimal promedioTotal = 0;

            using (var connection = new SqlConnection(_connectionStringBDPRACTICAS))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ENCUESTA.sp_ListarReporteEncuestaAsesor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new ReportePlano
                            {
                                TITULO_BLOQUE = reader["TITULO_BLOQUE"]?.ToString() ?? "(Sin bloque)",
                                TEXTO_PREGUNTA = reader["TEXTO_PREGUNTA"]?.ToString() ?? string.Empty,
                                PROMEDIO_SCORE = reader["PROMEDIO_SCORE"] != DBNull.Value ? Convert.ToDecimal(reader["PROMEDIO_SCORE"]) : (decimal?)null
                            });
                        }

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            promedioTotal = reader["PROMEDIO_TOTAL_SCORE"] != DBNull.Value
                                ? Convert.ToDecimal(reader["PROMEDIO_TOTAL_SCORE"])
                                : 0;
                        }
                    }
                }
            }

            var bloques = rows
                .GroupBy(r => r.TITULO_BLOQUE)
                .Select(g => new ReporteBloque
                {
                    BloqueId = g.First().BLOQUE_ID,
                    TituloBloque = g.Key,
                    Preguntas = g.Select(p => new ReportePregunta
                    {
                        EncuestaPreguntaId = p.ENCUESTA_PREGUNTA_ID,
                        TextoPregunta = p.TEXTO_PREGUNTA,
                        Promedio = p.PROMEDIO_SCORE ?? 0
                    }).ToList(),
                    PromedioBloque = Math.Round(
                        g.Where(p => p.PROMEDIO_SCORE.HasValue)
                         .Select(p => p.PROMEDIO_SCORE!.Value)
                         .DefaultIfEmpty(0)
                         .Average(), 2)
                })
                .ToList();

            return new ReporteEncuesta
            {
                EncuestaId = encuestaId,
                TipoEncuestado = "Asesor",
                Resumen = new ReporteResumen { PromedioTotalScore = promedioTotal },
                Bloques = bloques
            };
        }
    }

    internal class ReportePlano
    {
        public int BLOQUE_ID { get; set; }
        public string TITULO_BLOQUE { get; set; } = string.Empty;
        public int? ENCUESTA_PREGUNTA_ID { get; set; }
        public string TEXTO_PREGUNTA { get; set; } = string.Empty;
        public decimal? PROMEDIO_SCORE { get; set; }
        public decimal? PCT_1 { get; set; }
        public decimal? PCT_2 { get; set; }
        public decimal? PCT_3 { get; set; }
        public decimal? PCT_4 { get; set; }
        public decimal? PCT_5 { get; set; }
    }
}
