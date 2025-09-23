using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaAsignaturaCreateDTO
    {
        public int encuestaId { get; set; }
        public string NombreEncuesta { get; set; } = null!;
        public string DescripcionEncuesta { get; set; } = null!;
        public string TipoProgramaId { get; set; } = null!;
        public int TipoEncuestaId { get; set; }
        public string SedeId { get; set; } = null!;
        public string PeriodoId { get; set; } = null!;
        public string SeccionId { get; set; } = null!;
        public string NRC { get; set; } = null!;
        public string Asignatura { get; set; } = null!;
        public string Modulo { get; set; } = null!;
        public string Docente { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string userEmail { get; set; } = null!;
            
        public List<BloqueCreateDTO> Bloques { get; set; } = new List<BloqueCreateDTO>();
    }
}
