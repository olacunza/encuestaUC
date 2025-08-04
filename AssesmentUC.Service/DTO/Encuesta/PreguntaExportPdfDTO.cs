using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class PreguntaExportPdfDTO
    {
        public string TextoPregunta { get; set; } = null!;
        public string TipoPregunta { get; set; } = null!;
        public int Orden { get; set; }
    }
}
