using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CentroEmpEntidades;
using CentroEmpData.Contrato;
using CentroEmpWeb.Models.DTOs;

namespace CentroEmpWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;
        public AccesoController(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            if (claimuser.Identity!.IsAuthenticated)
            {
                string rol = claimuser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault()!;
                if (rol == "Administrador") return RedirectToAction("Index","Home","Citas");

                if (rol == "Emprendedor") return RedirectToAction("Index", "Citas");
                if (rol == "asesor") return RedirectToAction("CitasAsignadas", "Citas");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Console.WriteLine($"=== INTENTO DE LOGIN ===");
            Console.WriteLine($"Documento: {modelo.DocumentoIdentidad}");
            Console.WriteLine($"Contraseña: {modelo.Clave}");
            
            if (modelo.DocumentoIdentidad == null || modelo.Clave == null)
            {
                Console.WriteLine("ERROR: Documento o contraseña son null");
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            Console.WriteLine("Llamando al repositorio...");
            Usuario usuario_encontrado = await _repositorio.Login(modelo.DocumentoIdentidad, modelo.Clave);
            Console.WriteLine($"Usuario encontrado: {(usuario_encontrado != null ? "SÍ" : "NO")}");

            if (usuario_encontrado == null)
            {
                Console.WriteLine("ERROR: Usuario no encontrado en el repositorio");
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            //aqui guarderemos la informacion de nuestro usuario
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, $"{usuario_encontrado.Nombre} {usuario_encontrado.Apellido}"),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role,usuario_encontrado.RolUsuario.Nombre)
            };


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            string rol = usuario_encontrado.RolUsuario.Nombre;
            Console.WriteLine(usuario_encontrado.RolUsuario.IdRolUsuario);
            if (rol == "Emprendedor") return RedirectToAction("Index", "Citas");
            if (rol == "asesor") return RedirectToAction("CitasAsignadas", "Citas");

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Registrarse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrarse(VMEmprendedor modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewBag.Mensaje = "Las contraseñas no coinciden";
                return View();
            }

            Usuario objeto = new Usuario()
            {
                NumeroDocumentoIdentidad = modelo.DocumentoIdentidad,
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                RolUsuario = new RolUsuario()
                {
                    IdRolUsuario = 2
                }
            };
            string resultado = await _repositorio.Guardar(objeto);
            ViewBag.Mensaje = resultado;
            if (resultado == "")
            {
                ViewBag.Creado = true;
                ViewBag.Mensaje = "Su cuenta ha sido creada.";
            }
            return View();
        }

        public IActionResult Denegado()
        {
            return View();
        }
    }
}
