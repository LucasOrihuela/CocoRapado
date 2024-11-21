using Cocorapado.Models;
using Cocorapado.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Cocorapado.Controllers
{
    [Route("api/[controller]")]
    public class SucursalesController : Controller // Mantén Controller para acceso a View y Json
    {
        private readonly SucursalService _sucursalService;
        private readonly ProfesionalesPorSucursalService _profesionalesPorSucursalService;
        private readonly UsuarioService _usuarioService;

        public SucursalesController(SucursalService sucursalService, ProfesionalesPorSucursalService profesionalesPorSucursalService, UsuarioService usuarioService)
        {
            _sucursalService = sucursalService;
            _profesionalesPorSucursalService = profesionalesPorSucursalService;
            _usuarioService = usuarioService;
        }

        [HttpGet("GetServiciosBySucursal/{idSucursal}")]
        public async Task<IActionResult> GetServiciosBySucursal(int idSucursal)
        {
            try
            {
                var servicios = await _sucursalService.GetServiciosBySucursalIdAsync(idSucursal);
                return Json(servicios); // Devuelve resultado JSON
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener los servicios", error = ex.Message });
            }
        }

        [HttpPost("GetProfesionalesBySucursalAndServicios")]
        public async Task<IActionResult> GetProfesionalesBySucursalAndServicios([FromBody] ProfesionalesRequest request)
        {
            if (request == null || request.IdSucursal <= 0)
            {
                return BadRequest(new { message = "Parámetros inválidos" });
            }

            try
            {
                var profesionales = await _profesionalesPorSucursalService.ObtenerProfesionalesPorSucursalYServicioAsync(request.IdSucursal, request.IdsServicios ?? new List<int>());

                // Si no hay profesionales asignados al servicio, devolvemos una lista vacía
                if (!profesionales.Any())
                {
                    return Json(new List<object>());
                }

                return Json(profesionales);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener los profesionales", error = ex.Message });
            }
        }

        // Nueva acción para mostrar la vista de sucursal
        [HttpGet("{id}")]
        public async Task<IActionResult> Sucursal(int id)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            ViewBag.UsuarioId = usuarioId;
            var sucursal = await _sucursalService.GetSucursalByIdAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal); // Devuelve vista
        }

        [HttpPost("SuscribirClienteAbono")]
        public async Task<IActionResult> SuscribirClienteAbono([FromBody] SuscripcionRequest request)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                // Usuario no autenticado
                return Unauthorized(new { success = false, message = "Debe estar logeado para utilizar esta función." });
            }

            if (request == null || request.IdSucursal <= 0)
            {
                return BadRequest(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                int idCliente = int.Parse(usuarioId); // Convierte el UsuarioId de la sesión a entero
                var resultado = await _usuarioService.SuscribirClienteAbono(idCliente, request.IdSucursal);

                if (resultado)
                {
                    return Json(new { success = true, message = "Suscripción realizada con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo realizar la suscripción. Intente nuevamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error en el servidor.", error = ex.Message });
            }
        }


        public class SuscripcionRequest
        {
            public int IdSucursal { get; set; }
        }

        public class ProfesionalesRequest
        {
            public int IdSucursal { get; set; }
            public List<int>? IdsServicios { get; set; }
        }
    }
}
