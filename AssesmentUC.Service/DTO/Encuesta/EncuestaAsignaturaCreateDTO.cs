using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaAsignaturaCreateDTO
    {
        public int EncuestaId { get; set; }
        public string TipoPrograma { get; set; } = null!;
        public int TipoEncuestadoId { get; set; }
        public string Sede { get; set; } = null!;
        public string PeriodoId { get; set; } = null!;
        public string SeccionId { get; set; } = null!;
        public string NRC { get; set; } = null!;
        public string AsignaturaNombre { get; set; } = null!;
        public string Docente { get; set; } = null!;
        public string AsesorId { get; set; } = string.Empty;
        public string Asesor { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
