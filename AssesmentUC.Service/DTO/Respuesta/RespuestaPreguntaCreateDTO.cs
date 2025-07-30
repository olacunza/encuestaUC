using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Respuesta
{
    public class RespuestaPreguntaCreateDTO
    {
        public int EncuestaPreguntaId { get; set; }
        public string? ValorRespuesta { get; set; }
    }
}
