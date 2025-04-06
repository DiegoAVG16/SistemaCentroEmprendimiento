using CentroEmpEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpData.Contrato
{
    public interface IEspecialidadRepositorio
    {
        Task<List<Especialidad>> Lista();
        Task<string> Guardar(Especialidad objeto);
        Task<string> Editar(Especialidad objeto);
        Task<int> Eliminar(int Id);
    }
}
