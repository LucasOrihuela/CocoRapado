using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Cocorapado.Models;

namespace Cocorapado.Controllers
{
    public class MisReservasController : Controller
    {
        private readonly TurnoService _turnoService;
        private readonly UsuarioService _usuarioService;

        public MisReservasController(TurnoService turnoService, UsuarioService usuarioService)
        {
            _turnoService = turnoService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener el ID del usuario desde la sesión
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            var usuarioEmail = HttpContext.Session.GetString("UsuarioEmail");

            if (string.IsNullOrEmpty(usuarioIdString) || string.IsNullOrEmpty(usuarioEmail) || !int.TryParse(usuarioIdString, out var usuarioId))
            {
                // Redirige al usuario al inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Account");
            }

            // Obtener las reservas del usuario
            var reservas = await ObtenerReservasPorUsuario(usuarioId);

            // Obtener la información del usuario
            var usuario = await _usuarioService.ObtenerUsuarioPorCorreoAsync(usuarioEmail); // Método para obtener usuario

            // Verifica si el usuario existe
            if (usuario == null)
            {
                // Redirige al inicio si el usuario no existe
                return RedirectToAction("Index", "Home");
            }

            // Obtener el perfil del usuario
            var perfil = await _usuarioService.ObtenerPerfilPorUsuarioAsync(usuarioId); // Método para obtener perfil del usuario

            // Verifica si el perfil del usuario es válido
            if (perfil == null)
            {
                // Redirige al inicio si el perfil no existe
                return RedirectToAction("Index", "Home");
            }

            // Asigna el perfil al usuario
            usuario.Perfil = perfil;

            // Preparar el modelo para la vista
            var modelo = new MisReservasViewModel
            {
                Reservas = reservas,
                Usuario = usuario // El usuario tiene ahora el perfil asignado
            };

            return View("~/Views/Account/MisReservas.cshtml", modelo);
        }

        private async Task<IEnumerable<Turno>> ObtenerReservasPorUsuario(int usuarioId)
        {
            // Aquí podrías implementar un método para obtener las reservas específicas del usuario
            // Suponiendo que tienes un procedimiento almacenado para esto
            var todasLasReservas = await _turnoService.GetTurnosPorClienteAsync(usuarioId);
            return todasLasReservas; // Filtrar por usuario (ajustar según lógica real)
        }

        [HttpPost]
        public async Task<IActionResult> CancelarTurno(int turnoId)
        {
            try
            {
                // Llamar al servicio para cancelar el turno
                bool exito = await _turnoService.CancelarTurnoAsync(turnoId);

                if (exito)
                {
                    // Si la cancelación fue exitosa, retornar un mensaje de éxito
                    return Json(new { exito = true });
                }
                else
                {
                    // Si hubo un error al cancelar, retornar un mensaje de error
                    return Json(new { exito = false });
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error inesperado, retornar error
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }


    }

    // Modelo de la vista
    public class MisReservasViewModel
    {
        public IEnumerable<Turno> Reservas { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
