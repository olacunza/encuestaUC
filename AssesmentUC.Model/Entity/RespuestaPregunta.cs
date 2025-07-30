using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class RespuestaPregunta
    {
        public int RespuestaPreguntaId { get; set; }
        public int RespuestaEncuestaId { get; set; }
        public RespuestaEncuesta RespuestaEncuesta { get; set; } = null!;
        public int EncuestaPreguntaId { get; set; }
        public EncuestaPregunta EncuestaEncuesta { get; set; } = null!;
        public string? ValorRespuesta { get; set; }
    }
}
