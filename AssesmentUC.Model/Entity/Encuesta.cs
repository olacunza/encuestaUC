using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class Encuesta
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = null!;
        public string DescripcionEncuesta { get; set; } = null!;
        public int TipoProgramaId { get; set; }
        public string TipoPrograma { get; set; } = null!;
        public int TipoEncuestaId { get; set; }
        public string NombreTipoEncuesta { get; set; } = null!;
        public string SedeId { get; set; } = null!;
        public string Sede { get; set; } = null!;
        public string PeriodoId { get; set; } = null!;
        public string Periodo { get; set; } = null!;
        public string SeccionId { get; set; } = null!;
        public string Seccion { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Completado { get; set; }
        public bool Activo { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
        public ICollection<EncuestaBloque> Bloques { get; set; } = new List<EncuestaBloque>();
        
    }
}
