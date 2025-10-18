using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Reporte
{
    public class ReporteEncuestaDTO
    {
        public class ReporteEncuestaDto
        {
            public int EncuestaId { get; set; }
            public string TipoEncuestado { get; set; } = string.Empty;
            public List<ReporteBloqueDto> Bloques { get; set; } = new();
            public ReporteResumenDto? Resumen { get; set; }
        }

        public class ReporteBloqueDto
        {
            public int BloqueId { get; set; }
            public string TituloBloque { get; set; } = string.Empty;
            public List<ReportePreguntaDto> Preguntas { get; set; } = new();
            public decimal? PromedioBloque { get; set; }
        }

        public class ReportePreguntaDto
        {
            public int? EncuestaPreguntaId { get; set; }
            public string TextoPregunta { get; set; } = string.Empty;
            public decimal Promedio { get; set; }

            public decimal? Pct1 { get; set; }
            public decimal? Pct2 { get; set; }
            public decimal? Pct3 { get; set; }
            public decimal? Pct4 { get; set; }
            public decimal? Pct5 { get; set; }
        }

        public class ReporteResumenDto
        {
            public decimal? PromedioTotalScore { get; set; }
            public int? CantidadMatriculados { get; set; }
            public int? CantidadEncuestados { get; set; }
            public decimal? PorcentajeEncuestados { get; set; }
        }

    }
}
