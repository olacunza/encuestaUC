using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaDetailDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = null!;
        public string TipoPrograma { get; set; } = null!;
        public string TipoEncuesta { get; set; } = null!;
        public string Sede { get; set; } = null!;
        public string Periodo { get; set; } = null!;
        public string Seccion { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Completado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<BloqueDetailDTO> Bloques { get; set; } = new List<BloqueDetailDTO>();

    }
}
