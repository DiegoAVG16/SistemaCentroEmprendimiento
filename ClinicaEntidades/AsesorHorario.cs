using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpEntidades
{
    public class AsesorHorario
    {
        public int IdAsesorHorario { get; set; }
        public Asesor Asesor { get; set; } = null!;
        public int NumeroMes { get; set; }
        public string HoraInicioAM { get; set; } = null!;
        public string HoraFinAM { get; set; } = null!;
        public string HoraInicioPM { get; set; } = null!;
        public string HoraFinPM { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;
        public AsesorHorarioDetalle AsesorHorarioDetalle { get; set; } = null!;

    }
}
