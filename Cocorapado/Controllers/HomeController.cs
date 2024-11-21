using Cocorapado.Models;
using Cocorapado.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cocorapado.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SucursalService _sucursalService;
        private readonly UsuarioService _usuarioService;

        public HomeController(ILogger<HomeController> logger, SucursalService sucursalService, UsuarioService usuarioService)
        {
            _logger = logger;
            _sucursalService = sucursalService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Obtener las sucursales
                var sucursales = await _sucursalService.GetSucursalesAsync();
                var profesionales = await _usuarioService.GetTodosLosProfesionalesAsync();
                var profesionalesFavoritos = new List<int>();
                if (IsUserAuthenticatedAsClient())
                {
                    var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
                    if(usuarioIdString != null)
                    {
                        int usuarioId = int.Parse(usuarioIdString);
                        profesionalesFavoritos = await _usuarioService.GetProfesionalesFavoritosAsync(usuarioId);
                    }
                }                

                // Crear el ViewModel
                var viewModel = new IndexViewModel
                {
                    Sucursales = sucursales,
                    Profesionales = profesionales,
                    ProfesionalesFavoritos = profesionalesFavoritos
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sucursales.");
                return View(new IndexViewModel());
            }
        }

        private bool IsUserAuthenticatedAsClient()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Cliente";
        }

        [HttpPost]
        public IActionResult VerificarSesion()
        {
            if (IsUserAuthenticatedAsClient())
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarFavorito(int idProfesional)
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(idUsuario))
            {
                return Unauthorized("Debe estar logeado para agregar un profesional a favoritos.");
            }

            var idCliente = int.Parse(idUsuario); // Convertir el idUsuario a int
            var resultado = await _usuarioService.CrearProfesionalFavoritoAsync(idCliente, idProfesional);

            if (resultado)
            {
                return Ok(new { mensaje = "Profesional agregado a favoritos" });
            }

            return BadRequest("No se pudo agregar el profesional a favoritos.");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarFavorito(int idProfesional)
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(idUsuario))
            {
                return Unauthorized("Debe estar logeado para eliminar un profesional de favoritos.");
            }

            var idCliente = int.Parse(idUsuario); // Convertir el idUsuario a int
            var resultado = await _usuarioService.EliminarProfesionalFavoritoAsync(idCliente, idProfesional);

            if (resultado)
            {
                return Ok(new { mensaje = "Profesional eliminado de favoritos" });
            }

            return BadRequest("No se pudo eliminar el profesional de favoritos.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
