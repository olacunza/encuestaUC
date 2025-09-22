using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Abstractions;

namespace AssesmentUC.Service.DTO.Encuesta
{
    public class EncuestaListAllDTO
    {
        public int EncuestaId { get; set; }
        public string NombreEncuesta { get; set; } = string.Empty;
        public string DescripcionEncuesta { get; set; } = string.Empty;
        public string NombreTipoEncuesta { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    }
}
