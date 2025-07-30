using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Respuesta
{
    public class RespuestaEncuestaCreateDTO
    {
        public int EncuestaId { get; set; }
        public string AlumnoId { get; set; } = null!;
        public bool Completado { get; set; }
        public List<RespuestaPreguntaCreateDTO> Respuestas { get; set; } = new List<RespuestaPreguntaCreateDTO>();
    }
}
