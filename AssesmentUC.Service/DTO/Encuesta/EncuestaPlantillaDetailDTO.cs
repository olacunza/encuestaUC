using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaPlantillaDetailDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = null!;
        public string DescripcionEncuesta { get; set; } = null!;    
        public int TipoEncuestaId { get; set; }
        public string TipoEncuesta { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public List<BloqueDetailDTO> Bloques { get; set; } = new List<BloqueDetailDTO>();

    }
}
