using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class EncuestaBloque
    {
        public int BloqueId { get; set; }
        public string TituloBloque { get; set; } = string.Empty;
        public int OrdenBloque { get; set; }
        public bool Estado { get; set; }
        public int EncuestaId { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Encuesta Encuesta { get; set; } = null!;
        public ICollection<EncuestaPregunta> Preguntas { get; set; } = new List<EncuestaPregunta>();

    }
}
