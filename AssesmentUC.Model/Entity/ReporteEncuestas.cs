using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class ReporteEncuestas
    {        
        public class ReporteEncuesta
        {
            public int EncuestaId { get; set; }
            public string? TipoEncuestado { get; set; }
            public string NombreEncuesta { get; set; } = string.Empty;
            public string Docente { get; set; } = string.Empty;
            public string Periodo { get; set; } = string.Empty;
            public string Seccion { get; set; } = string.Empty;
            public string Asignatura { get; set; } = string.Empty;
            public string Programa { get; set; } = string.Empty;
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public List<ReporteBloque> Bloques { get; set; } = new();
            public ReporteResumen? Resumen { get; set; }
        }

        public class ReporteBloque
        {
            public int BloqueId { get; set; }
            public string TituloBloque { get; set; } = string.Empty;
            public List<ReportePregunta> Preguntas { get; set; } = new();
            public decimal? PromedioBloque { get; set; }
        }

        public class ReportePregunta
        {
            public int? EncuestaPreguntaId { get; set; }
            public string TextoPregunta { get; set; } = string.Empty;
            public decimal Promedio { get; set; }

            public decimal? PCT1 { get; set; }
            public decimal? PCT2 { get; set; }
            public decimal? PCT3 { get; set; }
            public decimal? PCT4 { get; set; }
            public decimal? PCT5 { get; set; }
            public decimal PromedioScore { get; set; }
        }

        public class ReporteResumen
        {
            public decimal? PromedioTotalScore { get; set; }
            public int? CantidadMatriculados { get; set; }
            public int? CantidadEncuestados { get; set; }
            public decimal? PorcentajeEncuestados { get; set; }
        }

    }
}
