using Cocorapado.Service;
using Cocorapado.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cocorapado.Controllers
{
    public class ABMServiciosController : Controller
    {
        private readonly ServicioService _servicioService;

        public ABMServiciosController(ServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        private bool IsUserAuthenticatedAsClient()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
        }

        // Acción Index para listar servicios
        public async Task<IActionResult> Index()
        {

            if (!IsUserAuthenticatedAsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var servicios = await _servicioService.GetTodosLosServiciosAsync();

                // Comprobación de servicios
                if (servicios == null || !servicios.Any())
                {
                    return NotFound("No hay servicios registrados.");
                }

                return View("~/Views/ABM/ABMServicios.cshtml", servicios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Acción para crear un nuevo servicio
        [HttpPost]
        public async Task<IActionResult> CrearServicio(Servicio nuevoServicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _servicioService.CrearServicioAsync(nuevoServicio);
                    return Json(new { success = true, message = "Servicio creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error interno al crear servicio: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Datos inválidos." });
        }

        // Acción para eliminar un servicio
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var servicioExistente = await _servicioService.ObtenerServicioPorIdAsync(id);
                if (servicioExistente == null)
                {
                    return Json(new { success = false, message = "Servicio no encontrado." });
                }

                var result = await _servicioService.EliminarServicioAsync(id);
                return result
                    ? Json(new { success = true, message = "El servicio se eliminó correctamente." })
                    : Json(new { success = false, message = "No se pudo eliminar el servicio." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        // Acción para editar un servicio
        [HttpPost]
        public async Task<IActionResult> Edit(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var servicioExistente = await _servicioService.ObtenerServicioPorIdAsync(servicio.Id);

                    if (servicioExistente == null)
                    {
                        return Json(new { success = false, message = "Servicio no encontrado." });
                    }

                    // Llamar al servicio para editar
                    var result = await _servicioService.EditarServicioAsync(servicio);

                    return Json(new
                    {
                        success = result,
                        message = result ? "El servicio se editó correctamente." : "No se pudo editar el servicio."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Datos del servicio no son válidos." });
        }

        // Acción para obtener un servicio por ID
        [HttpGet]
        public async Task<JsonResult> ObtenerServicio(int id)
        {
            var servicio = await _servicioService.ObtenerServicioPorIdAsync(id);

            if (servicio != null)
            {
                return Json(new
                {
                    id = servicio.Id,
                    nombre = servicio.ServicioNombre,
                    descripcion = servicio.ServicioDescripcion,
                    duracion = servicio.DuracionMinutos,
                    precioMin = servicio.PrecioMin,
                    precioMax = servicio.PrecioMax,
                    imagen = servicio.Imagen
                });
            }

            return Json(new { success = false, message = "Servicio no encontrado." });
        }
    }
}
