using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaAsignaturaFiltroDTO
    {
        public string Seccion { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.MinValue;
        public DateTime FechaFin { get; set; } = DateTime.MinValue;
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
