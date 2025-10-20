using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.ViewModel
{
    public class ReporteEncuestaAlumnoViewModel
    {
        public string? NombreEncuesta { get; set; }
        public string? Programa { get; set; }
        public string? Seccion { get; set; }
        public string? Asignatura { get; set; }
        public string? Docente { get; set; }
        public ReporteResumenViewModel? Resumen { get; set; }
        public List<ReporteBloqueViewModel> Bloques { get; set; } = new();
    }

    public class ReporteResumenViewModel
    {
        public int CantidadMatriculados { get; set; }
        public int CantidadEncuestados { get; set; }
        public decimal PorcentajeEncuestados { get; set; }
        public decimal PromedioTotalScore { get; set; }
    }

    public class ReporteBloqueViewModel
    {
        public int BloqueId { get; set; }
        public string? TituloBloque { get; set; }
        public List<ReportePreguntaViewModel> Preguntas { get; set; } = new();
        public decimal? PromedioBloque { get; set; }
    }

    public class ReportePreguntaViewModel
    {
        public string TextoPregunta { get; set; } = string.Empty;
        public decimal Promedio { get; set; }
        public decimal? Pct1 { get; set; }
        public decimal? Pct2 { get; set; }
        public decimal? Pct3 { get; set; }
        public decimal? Pct4 { get; set; }
        public decimal? Pct5 { get; set; }
    }
}
