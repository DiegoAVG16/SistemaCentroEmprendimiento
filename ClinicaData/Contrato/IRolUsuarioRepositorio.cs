using CentroEmpEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpData.Contrato
{
    public interface IRolUsuarioRepositorio
    {
        Task<List<RolUsuario>> Lista();
    }
}
