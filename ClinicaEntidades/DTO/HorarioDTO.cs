using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpEntidades.DTO
{
    public class HorarioDTO
    {
        public int IdAsesorHorarioDetalle { get; set; }
        public string Turno { get; set; } = null!;
        public string TurnoHora { get; set; } = null!;
    }
}
