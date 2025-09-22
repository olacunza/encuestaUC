using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaPlantillaCreateDTO
    {
        public string NombreEncuesta { get; set; } = null!;
        public string DescripcionEncuesta { get; set; } = null!;
        public int TipoEncuestaId { get; set; }

        public List<BloqueCreateDTO> Bloques { get; set; } = new List<BloqueCreateDTO>();
    }
}
