using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class CrearEncuestaAsignaturaRequestDTO
    {
        public EncuestaAsignaturaCreateDTO Encuesta { get; set; } = null!;
        public EncuestaDatosCorreoDTO DatosCorreo { get; set; } = null!;
        public string Usuario { get; set; } = null!;
    }
}
