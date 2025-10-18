using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class ReportePlano
    {
        public int NRO { get; set; }
        public string TITULO_BLOQUE { get; set; } = string.Empty;
        public int ENCUESTA_PREGUNTA_ID { get; set; }
        public string TEXTO_PREGUNTA { get; set; } = string.Empty;
        public decimal? PROMEDIO_SCORE { get; set; }
    }

}
