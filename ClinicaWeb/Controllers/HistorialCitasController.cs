using CentroEmpData.Contrato;
using CentroEmpEntidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroEmpWeb.Controllers
{
    public class HistorialCitasController : Controller
    {
        private readonly ICitaRepositorio _repositorioCita;
        public HistorialCitasController(ICitaRepositorio repositorioCita)
        {
            _repositorioCita = repositorioCita;
        }
        [Authorize(Roles = "Emprendedor")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaHistorialCitas()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            string idUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!;
            Console.WriteLine("usuario id: " + idUsuario);
            //List<Cita> lista = await _repositorioCita.ListaHistorialCitas(int.Parse(idUsuario));
            List<Cita> lista = await _repositorioCita.ListaHistorialCitas(int.Parse(idUsuario));

            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }
    }
}
