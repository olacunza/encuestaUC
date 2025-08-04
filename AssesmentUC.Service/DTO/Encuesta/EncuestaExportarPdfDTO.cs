using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaExportarPdfDTO
    {
        public string NombreEncuesta { get; set; } = null!;
        public string? DescripcionEncuesta { get; set; }
        public string? Periodo { get; set; } = null!;
        public string? Seccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<BloqueExportPdfDTO> Bloques { get; set; } = new List<BloqueExportPdfDTO>();
    }
}
