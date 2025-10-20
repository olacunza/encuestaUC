using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.ViewModel
{
    public class ReporteEncuestaAsesorViewModel
    {
        public string Programa { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;

        public List<PreguntaViewModel> Preguntas { get; set; } = new();
        public decimal? PromedioArea { get; set; }
        public decimal? PromedioEstudiantes { get; set; }
        public decimal? ModeloAcademico { get; set; }
        public decimal? EvaluacionFinal { get; set; }

        public string? Comentarios { get; set; }
    }

    public class PreguntaViewModel
    {
        public int Nro { get; set; }
        public string TextoPregunta { get; set; } = string.Empty;
        public decimal Promedio { get; set; }
    }
}
