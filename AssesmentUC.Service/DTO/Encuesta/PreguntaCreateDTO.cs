using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class PreguntaCreateDTO
    {
        public string TextoPregunta { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public int OrdenPregunta { get; set; }
        public string? OpcionesJson { get; set; }
    }
}
