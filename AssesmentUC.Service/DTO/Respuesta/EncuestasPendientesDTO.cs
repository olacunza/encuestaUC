using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Respuesta
{
    public class EncuestasPendientesDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = string.Empty;
        public string DescripcionEncuesta { get; set; } = string.Empty;
        public string NombreTipoEncuesta { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
