using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Respuesta
{
    public class RespuestaListAllDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = null!;
        public DateTime FechaRespuesta { get; set; }
        public bool Completado { get; set; }
    }
}
