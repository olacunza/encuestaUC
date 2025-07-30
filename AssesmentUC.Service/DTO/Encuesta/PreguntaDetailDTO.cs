using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class PreguntaDetailDTO
    {
        public int PreguntaId { get; set; }
        public string TextoPregunta { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public int Orden { get; set; }
        public string? OpcionesJson { get; set; }
    }
}
