using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class ReporteEncuestaExcel
    {
        public Hoja1ResumenEncuesta Hoja1 { get; set; } = new();
        public List<Hoja2RespuestasPivot> Hoja2 { get; set; } = new();
        public Hoja3ResumenFinal Hoja3 { get; set; } = new();
    }

    public class Hoja1ResumenEncuesta
    {
        public string Programa { get; set; }
        public string Asignatura { get; set; }
        public string Docente { get; set; }
        public string Fechas { get; set; }
        public string TipoEncuesta { get; set; }

        public List<Hoja1Bloque> Bloques { get; set; } = new();
        public decimal PromedioTotal { get; set; }
    }

    public class Hoja1Bloque
    {
        public int BloqueId { get; set; }
        public string TituloBloque { get; set; }
        public decimal PromedioBloque { get; set; }
        public List<Hoja1Pregunta> Preguntas { get; set; } = new();
    }

    public class Hoja1Pregunta
    {
        public int PreguntaId { get; set; }
        public string TextoPregunta { get; set; }
        public string TipoPregunta { get; set; }
        public decimal PromedioPregunta { get; set; }
        public List<string> OptionLabels { get; set; }            // ej: ["1","2","3","4","5"] o textos
        public List<decimal?> OptionPercentages { get; set; }     // ej: [10.0m, 0, 30.0m, 40.0m, 20.0m]
    }


    public class Hoja2RespuestasPivot
    {
        public int EncuestaId { get; set; }
        public Dictionary<int, decimal?> Respuestas { get; set; } = new(); // clave: PREGUNTA_ID
        public string? Comentario { get; set; }
    }

    public class Hoja3ResumenFinal
    {
        public int CantidadEncuestados { get; set; }
    }

}
