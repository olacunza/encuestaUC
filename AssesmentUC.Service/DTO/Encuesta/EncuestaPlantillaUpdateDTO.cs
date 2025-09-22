using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaPlantillaUpdateDTO
    {
        public int EncuestaId { get; set; }
        public string? NombreEncuesta { get; set; }
        public string? DescripcionEncuesta { get; set; }
        public int? TipoEncuestaId { get; set; }
        public List<BloqueUpdateDTO> Bloques { get; set; } = new List<BloqueUpdateDTO>();
    }
}
