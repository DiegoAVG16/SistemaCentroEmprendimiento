using CentroEmpEntidades;
using CentroEmpEntidades.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroEmpData.Contrato
{
    public interface IAsesorRepositorio
    {
        Task<List<Asesor>> Lista();
        Task<string> Guardar(Asesor objeto);
        Task<string> Editar(Asesor objeto);
        Task<int> Eliminar(int Id);

        Task<string> RegistrarHorario(AsesorHorario objeto);
        Task<List<AsesorHorario>> ListaAsesorHorario();        
        Task<string> EliminarHorario(int Id);
        Task<List<FechaAtencionDTO>> ListaAsesorHorarioDetalle(int Id);
        Task<List<Cita>> ListaCitasAsignadas(int Id, int IdEstadoCita);
        Task<List<AsesorHorario>> ListaAsesorHorario(int id);


    }
}
