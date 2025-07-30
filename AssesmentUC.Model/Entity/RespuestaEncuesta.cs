using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class RespuestaEncuesta
    {
        public int RespuestaEncuestaId { get; set; }
        public int EncuestaId { get; set; }
        public Encuesta Encuesta { get; set; } = null!;
        public string AlumnoId { get; set; } = null!;
        public DateTime FechaRespuesta { get; set; }
        public bool Completado { get; set; }
        public ICollection<RespuestaPregunta> Respuestas { get; set; } = new List<RespuestaPregunta>();
    }
}
