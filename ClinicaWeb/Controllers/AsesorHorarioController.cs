using CentroEmpData;
using CentroEmpData.Contrato;
using CentroEmpEntidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroEmpWeb.Controllers
{
    public class AsesorHorarioController : Controller
    {
        private readonly IAsesorRepositorio _repositorio;
        public AsesorHorarioController(IAsesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<AsesorHorario> lista = await _repositorio.ListaAsesorHorario();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        public async Task<IActionResult> Guardar([FromBody] AsesorHorario objeto)
        {
            string respuesta = await _repositorio.RegistrarHorario(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _repositorio.EliminarHorario(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

    }
}

