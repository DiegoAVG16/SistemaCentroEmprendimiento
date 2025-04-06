using CentroEmpData.Contrato;
using CentroEmpEntidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroEmpWeb.Controllers
{
    public class AsesorController : Controller
    {
        private readonly IAsesorRepositorio _repositorio;
        private readonly ICitaRepositorio _repositorioCita;
        public AsesorController(IAsesorRepositorio repositorio, ICitaRepositorio repositorioCita)
        {
            _repositorio = repositorio;
            _repositorioCita = repositorioCita;
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Emprendedor,Asesor")]
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Asesor> lista = await _repositorio.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Asesor objeto)
        {
            string respuesta = await _repositorio.Guardar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Asesor objeto)
        {
            string respuesta = await _repositorio.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            int respuesta = await _repositorio.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpGet]
        public async Task<IActionResult> ListaCitasAsignadas(int IdEstadoCita)
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            string idUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!;

            List<Cita> lista = await _repositorio.ListaCitasAsignadas(int.Parse(idUsuario),IdEstadoCita);
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] Cita objeto)
        {
            string respuesta = await _repositorioCita.CambiarEstado(objeto.IdCita,objeto.EstadoCita.IdEstadoCita,objeto.Indicaciones);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }
    }
}
