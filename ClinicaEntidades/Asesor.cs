using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpEntidades
{
    public class Asesor
    {
        public int IdAsesor { get; set; }
        public string NumeroDocumentoIdentidad { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public Especialidad Especialidad { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;

    }
}
