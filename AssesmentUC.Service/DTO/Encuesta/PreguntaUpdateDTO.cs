using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class PreguntaUpdateDTO
    {
        public int? PreguntaId { get; set; }
        public string? TextoPregunta { get; set; }
        public string? TipoPregunta { get; set; }
        public int? Orden { get; set; }
        public string? OpcionesJson { get; set; }
    }
}
