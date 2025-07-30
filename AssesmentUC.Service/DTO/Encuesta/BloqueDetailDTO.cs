using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class BloqueDetailDTO
    {
        public int BloqueId { get; set; }
        public string TituloBloque { get; set; } = null!;
        public int Orden { get; set; }
        public List<PreguntaDetailDTO> Preguntas { get; set; } = new List<PreguntaDetailDTO>();
    }
}
