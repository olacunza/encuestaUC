using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaDatosCorreoDTO
    {
        public string accessToken { get; set; } = null!;
        public string userEmail { get; set; } = null!;
        public int encuestaId { get; set; }
        public string linkEncuesta { get; set; } = null!;
        public string asignatura { get; set; } = null!;
        public string nombreEncuesta { get; set; } = null!;
        public string motivoCorreo { get; set; } = null!;
        public string cuerpoCorreo { get; set; } = null!;
        public string? AlumnoID { get; set; }
        public string? CorreoAlumno { get; set; }
    }
}
