using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaUpdateDTO
    {
        public int EncuestaId { get; set; }
        public string? NombreEncuesta { get; set; }
        public string? DescripcionEncuesta { get; set; }
        public int? TipoPrograma { get; set; }
        public int? Sede { get; set; }
        public string? Preiodo { get; set; }
        public string? Seccion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        public string UsuarioModificacion { get; set; } = null!;
        public DateTime FechaModificacion { get; set; }
        public List<BloqueUpdateDTO> Bloques { get; set; } = new List<BloqueUpdateDTO>();
    }
}
