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
        public string? TipoProgramaId { get; set; }
        public int? TipoEncuestaId { get; set; }
        public string? SedeId { get; set; }
        public string? PeriodoId { get; set; }
        public string? Seccion { get; set; }
        public bool Completado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        public List<BloqueUpdateDTO> Bloques { get; set; } = new List<BloqueUpdateDTO>();
    }
}
