using CentroEmpData.Contrato;
using CentroEmpEntidades;
using CentroEmpEntidades.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroEmpWeb.Controllers
{
    public class CitasController : Controller
    {
        private readonly IAsesorRepositorio _repositorio;
        private readonly ICitaRepositorio _repositorioCita;
        public CitasController(IAsesorRepositorio repositorio, ICitaRepositorio repositorioCita)
        {
            _repositorio = repositorio;
            _repositorioCita = repositorioCita;
        }

        [Authorize(Roles = "Emprendedor")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Emprendedor")]
        public IActionResult NuevaCita()
        {
            return View();
        }

        [Authorize(Roles = "asesor")]
        public IActionResult CitasAsignadas()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaAsesorHorarioDetalle(int Id)
        {
            List<FechaAtencionDTO> lista = await _repositorio.ListaAsesorHorarioDetalle(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Cita objeto)
        {

            ClaimsPrincipal claimuser = HttpContext.User;
            string idUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!;

            objeto.Usuario = new Usuario
            {
                IdUsuario = int.Parse(idUsuario)
            };

            string respuesta = await _repositorioCita.Guardar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpGet]
        public async Task<IActionResult> ListaCitasPendiente()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            string idUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!;

            List<Cita> lista = await _repositorioCita.ListaCitasPendiente(int.Parse(idUsuario));
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpDelete]
        public async Task<IActionResult> Cancelar(int Id)
        {
            string respuesta = await _repositorioCita.Cancelar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

       
    }
}
