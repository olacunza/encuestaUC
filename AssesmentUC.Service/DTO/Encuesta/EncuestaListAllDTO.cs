using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaListAllDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = string.Empty;
        public string DescripcionEncuesta { get; set; } = string.Empty;
        public int TipoPrograma { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
