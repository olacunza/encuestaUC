using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Model.Entity
{
    public class EnviarCorreoEncuesta
    {
        //public int EncuestaId { get; set; }
        //public string AccessToken { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string ToEmail { get; set; } = null!;
        public string NombreEncuesta { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
