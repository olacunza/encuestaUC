using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssesmentUC.Model.Entity.ReporteEncuestas;

namespace AssesmentUC.Service.DTO.ViewModel
{
    public class ReporteEncuestaDocenteViewModel
    {
        public string NombreEncuesta { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<ReporteBloque> Bloques { get; set; } = new();
    }

}
