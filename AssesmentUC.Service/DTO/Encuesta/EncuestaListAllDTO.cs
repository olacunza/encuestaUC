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
        public string Sede { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Programa { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
