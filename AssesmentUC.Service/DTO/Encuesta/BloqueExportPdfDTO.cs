using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class BloqueExportPdfDTO
    {
        public string TituloBloque { get; set; } = null!;
        public int Orden { get; set; }
        public List<PreguntaExportPdfDTO> Preguntas { get; set; } = new List<PreguntaExportPdfDTO>();
    }
}
