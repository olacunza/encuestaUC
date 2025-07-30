using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class BloqueCreateDTO
    {
        public string TituloBloque { get; set; } = string.Empty;
        public int OrdenBloque { get; set; }
        public List<PreguntaCreateDTO> Preguntas { get; set; } = new List<PreguntaCreateDTO>();
    }
}
