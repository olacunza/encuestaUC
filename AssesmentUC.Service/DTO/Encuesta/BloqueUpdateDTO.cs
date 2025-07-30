using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class BloqueUpdateDTO
    {
        public int? BloqueId { get; set; }
        public string? TituloBloque { get; set; }
        public int? Orden { get; set; }
        public List<PreguntaUpdateDTO> Preguntas { get; set; } = new List<PreguntaUpdateDTO>();
    }
}
