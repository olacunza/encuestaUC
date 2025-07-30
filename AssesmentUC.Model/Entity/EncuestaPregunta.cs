using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class EncuestaPregunta
    {
        public int EncuestaDetalleId;
        public string TextoPregunta { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public int OrdenPregunta { get; set; }
        public string? OpcionesJson { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
        public int BloqueId { get; set; }
        public EncuestaBloque EncuestaBloque { get; set; } = null!;
    }
}
