using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaCreateDTO
    {
        public string NombreEncuesta { get; set; } = null!;
        public string DescripcionEncuesta { get; set; } = null!;
        public int TipoPrograma { get; set; }
        public int Sede { get; set; }
        public string Periodo { get; set; } = null!;
        public string Seccion { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<BloqueCreateDTO> Bloques { get; set; } = new List<BloqueCreateDTO>();
    }
}
