using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
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
            var cabecera = new ReporteEncuesta();

            using (var connection = new SqlConnection(_connectionStringBDPRACTICAS))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ENCUESTA.sp_ListarReporteEncuestaAlumno", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cabecera.NombreEncuesta = reader["NOMBRE_ENCUESTA"]?.ToString() ?? "";
                            cabecera.Programa = reader["PROGRAMA"]?.ToString() ?? "";
                            cabecera.Seccion = reader["SECCION"]?.ToString() ?? "";
                            cabecera.Asignatura = reader["ASIGNATURA"]?.ToString() ?? "";
                            cabecera.Docente = reader["DOCENTE"]?.ToString() ?? "";
                        }

                        // --- 2️⃣ Segundo SELECT: preguntas y promedios ---
                        if (await reader.NextResultAsync())
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
                NombreEncuesta = cabecera.NombreEncuesta,
                Docente = cabecera.Docente,
                Periodo = cabecera.Periodo,
                Programa = cabecera.Programa,
                Seccion = cabecera.Seccion,
                Asignatura = cabecera.Asignatura,
                FechaInicio = cabecera.FechaInicio,
                FechaFin = cabecera.FechaFin,
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
            var cabeceraEncuesta = new ReporteEncuesta();
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
                        if (await reader.ReadAsync())
                        {
                            cabeceraEncuesta.NombreEncuesta = reader["NOMBRE_ENCUESTA"]?.ToString() ?? "";
                            cabeceraEncuesta.Programa = reader["PROGRAMA"]?.ToString() ?? "";
                            cabeceraEncuesta.Seccion = reader["SECCION"]?.ToString() ?? "";
                            cabeceraEncuesta.Asignatura = reader["ASIGNATURA"]?.ToString() ?? "";
                            cabeceraEncuesta.Docente = reader["DOCENTE"]?.ToString() ?? "";
                        }

                        if (await reader.NextResultAsync())
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

            cabeceraEncuesta.EncuestaId = encuestaId;
            cabeceraEncuesta.TipoEncuestado = "Asesor";
            cabeceraEncuesta.Resumen = new ReporteResumen { PromedioTotalScore = promedioTotal };
            cabeceraEncuesta.Bloques = bloques;

            return cabeceraEncuesta;
        }

        public async Task<ReporteEncuestaExcel> ExportarValoresEncuestaExcel(int encuestaId)
        {
            var reporte = new ReporteEncuestaExcel
            {
                Hoja1 = new Hoja1ResumenEncuesta(),
                Hoja2 = new List<Hoja2RespuestasPivot>()
            };

            using (var connection = new SqlConnection(_connectionStringBDPRACTICAS))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ENCUESTA.sp_ListarReporteParaExcel", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ENCUESTA_ID", encuestaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // ================== HOJA 1 - INFORMACIÓN GENERAL ==================
                        if (await reader.ReadAsync())
                        {
                            reporte.Hoja1 = new Hoja1ResumenEncuesta
                            {
                                Programa = reader["PROGRAMA"]?.ToString(),
                                Asignatura = reader["ASIGNATURA"]?.ToString(),
                                Docente = reader["DOCENTE"]?.ToString(),
                                Fechas = reader["FECHAS"]?.ToString(),
                                TipoEncuesta = reader["TIPO_ENCUESTA"]?.ToString()
                            };
                        }

                        // ================== BLOQUES PROMEDIO ==================
                        var bloques = new List<Hoja1Bloque>();
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                bloques.Add(new Hoja1Bloque
                                {
                                    BloqueId = reader["BLOQUE_ID"] != DBNull.Value ? Convert.ToInt32(reader["BLOQUE_ID"]) : 0,
                                    TituloBloque = reader["TITULO_BLOQUE"]?.ToString(),
                                    PromedioBloque = reader["PROMEDIO_BLOQUE"] != DBNull.Value ? Convert.ToDecimal(reader["PROMEDIO_BLOQUE"]) : 0,
                                    Preguntas = new List<Hoja1Pregunta>()
                                });
                            }
                        }

                        // ================== PREGUNTAS ==================
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int bloqueId = reader["BLOQUE_ID"] != DBNull.Value ? Convert.ToInt32(reader["BLOQUE_ID"]) : 0;
                                var bloque = bloques.FirstOrDefault(b => b.BloqueId == bloqueId);
                                if (bloque != null)
                                {
                                    bloque.Preguntas.Add(new Hoja1Pregunta
                                    {
                                        PreguntaId = reader["PREGUNTA_ID"] != DBNull.Value ? Convert.ToInt32(reader["PREGUNTA_ID"]) : 0,
                                        TextoPregunta = reader["TEXTO_PREGUNTA"]?.ToString(),
                                        TipoPregunta = reader["TIPO_PREGUNTA"]?.ToString(),
                                        PromedioPregunta = reader["PROMEDIO_PREGUNTA"] != DBNull.Value
                                            ? Convert.ToDecimal(reader["PROMEDIO_PREGUNTA"])
                                            : 0
                                    });
                                }
                            }
                        }

                        // Asegurarnos que Hoja1 no sea null
                        reporte.Hoja1.Bloques = bloques;

                        // ================== PROMEDIO TOTAL ==================
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            reporte.Hoja1.PromedioTotal = reader["PROMEDIO_TOTAL"] != DBNull.Value
                                ? Convert.ToDecimal(reader["PROMEDIO_TOTAL"])
                                : 0;
                        }

                        // ================== CABECERA RESPUESTAS (OPCIONAL) ==================
                        if (await reader.NextResultAsync())
                        {
                            // Este resultset parece ser la "cabecera" (ENCUESTA_ID, PREGUNTA_ID, TEXTO_PREGUNTA, COMENTARIO)
                            // Si no necesitas almacenarlo, simplemente consumirlo o ignorarlo.
                            while (await reader.ReadAsync())
                            {
                                // Si quieres guardarlo, hazlo aquí; de lo contrario lo consumimos y seguimos.
                            }
                        }

                        // ================== CONTENIDO PIVOT ==================
                        if (await reader.NextResultAsync())
                        {
                            var schema = reader.GetColumnSchema();
                            var columnasPreguntas = schema
                                .Where(c => int.TryParse(c.ColumnName, out _))
                                .Select(c => int.Parse(c.ColumnName))
                                .ToList();

                            while (await reader.ReadAsync())
                            {
                                var item = new Hoja2RespuestasPivot
                                {
                                    EncuestaId = reader["ENCUESTA_ID"] != DBNull.Value ? Convert.ToInt32(reader["ENCUESTA_ID"]) : 0,
                                    Comentario = reader["COMENTARIO"]?.ToString()
                                };

                                // Inicializar diccionario si es necesario
                                if (item.Respuestas == null) item.Respuestas = new Dictionary<int, decimal?>();

                                foreach (var pid in columnasPreguntas)
                                {
                                    var val = reader[pid.ToString()];
                                    item.Respuestas[pid] = val != DBNull.Value ? Convert.ToDecimal(val) : (decimal?)null;
                                }

                                reporte.Hoja2.Add(item);
                            }
                        }

                        // ----------------- IMPORTANTE: Después del PIVOT, el SP devuelve
                        //                   UN resultset con PREGUNTA_ID,TEXTO_PREGUNTA
                        //                   antes del resumen final. Hay que saltarlo. ----
                        if (await reader.NextResultAsync())
                        {
                            // Este NextResultAsync() llega al SELECT PREGUNTA_ID, TEXTO_PREGUNTA
                            // Si no necesitas ese resultset, consumimos/ignoramos:
                            while (await reader.ReadAsync()) { /* ignorar */ }
                        }

                        // ================== RESUMEN FINAL ==================
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            // Ahora sí estamos en el resultset del COUNT
                            reporte.Hoja3 = new Hoja3ResumenFinal
                            {
                                CantidadEncuestados = reader["CANTIDAD_ENCUESTADOS"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CANTIDAD_ENCUESTADOS"])
                                    : 0
                            };
                        }
                        else
                        {
                            // Si no vino resultset final, inicializamos por seguridad
                            reporte.Hoja3 = new Hoja3ResumenFinal { CantidadEncuestados = 0 };
                        }
                    }
                }
            }

            return reporte;
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
