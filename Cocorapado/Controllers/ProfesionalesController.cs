using Cocorapado.Service;
using Cocorapado.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cocorapado.Controllers
{
    public class ProfesionalesController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly TurnoService _turnoService;

        public ProfesionalesController(UsuarioService usuarioService, TurnoService turnoService)
        {
            _usuarioService = usuarioService;
            _turnoService = turnoService;
        }

        // Acción Index que recibe el id del servicio y sucursal
        public async Task<IActionResult> Index(int idServicio, int sucursalId)
        {
            var profesionalIdString = HttpContext.Session.GetString("UsuarioId");
            var profesionalEmail = HttpContext.Session.GetString("UsuarioEmail");

            if (string.IsNullOrEmpty(profesionalIdString) || string.IsNullOrEmpty(profesionalEmail) || !int.TryParse(profesionalIdString, out var profesionalId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var reservas = await ObtenerReservasPorProfesional(profesionalId);
                var profesional = await _usuarioService.ObtenerUsuarioPorCorreoAsync(profesionalEmail);
                var perfil = await _usuarioService.ObtenerPerfilPorUsuarioAsync(profesionalId);

                if (profesional == null || perfil.Rol != "Profesional")
                {
                    return RedirectToAction("Login", "Account");
                }

                profesional.Perfil = perfil;

                var modelo = new ReservasProfesionalViewModel
                {
                    Reservas = reservas,
                    Usuario = profesional
                };

                return View("~/Views/Profesional/ReservasProfesional.cshtml", modelo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClienteAusente(int idTurno)
        {
            if (idTurno <= 0)
            {
                return BadRequest(new { success = false, message = "ID de turno inválido." });
            }

            try
            {
                // Usa await porque MarcarClienteAusente es asíncrono
                bool resultado = await _turnoService.MarcarClienteAusente(idTurno);

                if (resultado)
                {
                    return Json(new { success = true, message = "Cliente marcado como ausente con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo marcar al cliente como ausente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error en el servidor.", error = ex.Message });
            }
        }


        private async Task<IEnumerable<Turno>> ObtenerReservasPorProfesional(int profesionalId)
        {
            // Aquí podrías implementar un método para obtener las reservas específicas del usuario
            // Suponiendo que tienes un procedimiento almacenado para esto
            var todasLasReservas = await _turnoService.GetTurnosPorProfesionalAsync(profesionalId);
            return todasLasReservas; // Filtrar por usuario (ajustar según lógica real)
        }
    }

    public class ReservasProfesionalViewModel
    {
        public IEnumerable<Turno> Reservas { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
