using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using Cocorapado.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ABMProfesionalesPorSucursalController : Controller
{
    private readonly ILogger<ABMProfesionalesPorSucursalController> _logger;
    private readonly SucursalService _sucursalService;
    private readonly ProfesionalesPorSucursalService _profesionalesPorSucursalService;
    private readonly UsuarioService _usuarioService;

    public ABMProfesionalesPorSucursalController(ILogger<ABMProfesionalesPorSucursalController> logger, ProfesionalesPorSucursalService ProfesionalesPorSucursalService, SucursalService sucursalService, UsuarioService usuarioService)
    {
        _logger = logger;
        _profesionalesPorSucursalService = ProfesionalesPorSucursalService;
        _sucursalService = sucursalService;
        _usuarioService = usuarioService;
    }

    private bool IsUserAuthenticatedAsClient()
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

        return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
    }

    // Acción para la vista de sucursales por profesional
    public async Task<IActionResult> Index()
    {

        if (!IsUserAuthenticatedAsClient())
        {
            return RedirectToAction("Login", "Account");
        }

        // Verifica que los sucursales no sean null antes de utilizarlos
        if (_profesionalesPorSucursalService == null || _sucursalService == null)
        {
            return StatusCode(500, "Error en la configuración de las sucursales.");
        }

        // Obtener relaciones de profesionales por sucursal utilizando el id de sucursal
        var relaciones = await _profesionalesPorSucursalService.ObtenerRelacionesAsync();

        // Maneja el caso en que no haya relaciones
        if (relaciones == null || relaciones.Count == 0)
        {
            relaciones = new List<ProfesionalesPorSucursales>();
        }

        // Obtener lista de sucursales para el dropdown
        var sucursales = await _sucursalService.GetSucursalesAsync();
        ViewBag.Sucursales = sucursales;

        // Retorna la vista con las relaciones
        return View("~/Views/ABM/ABMProfesionalesPorSucursal.cshtml", relaciones);
    }

    public async Task<IActionResult> ObtenerProfesionalesYSucursales()
    {
        // Obtener la lista de profesionales y proyectar solo el nombre y apellido
        var profesionales = await _usuarioService.GetTodosLosProfesionalesAsync();
        var profesionalesFiltrados = profesionales.Select(p => new
        {
            p.Id,
            p.Nombre,
            p.Apellido
        }).ToList();

        // Obtener la lista de sucursales y proyectar solo el nombre del sucursal
        var sucursales = await _sucursalService.GetSucursalesAsync();
        var sucursalesFiltradas = sucursales.Select(s => new
        {
            s.Id,
            s.Nombre
        }).ToList();

        // Devolver los datos como JSON
        return Json(new { profesionales = profesionalesFiltrados, sucursales = sucursalesFiltradas });
    }

    // Acción para obtener sucursales sin asignar para un profesional
    [HttpPost]
    public async Task<IActionResult> ObtenerSucursalesSinAsignar(int id_profesional)
    {
        try
        {
            // Llamar al método del sucursal que obtiene sucursales sin asignar
            var sucursalesSinAsignar = await _sucursalService.ObtenerSucursalesSinAsignarAsync(id_profesional);

            // Verifica si se obtuvieron sucursales
            if (sucursalesSinAsignar == null || !sucursalesSinAsignar.Any())
            {
                return Json(new { success = false, message = "No se encontraron sucursales para este profesional." });
            }

            // Proyectar los resultados a un formato más simple
            var sucursalesFiltradas = sucursalesSinAsignar.Select(s => new
            {
                id = s.Id,
                sucursal = s.Nombre
            }).ToList();

            return Json(new { success = true, sucursales = sucursalesSinAsignar });
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EliminarRelacion(int id_profesional, int id_sucursal)
    {
        try
        {
            // Llamar al método de la sucursal que ejecuta el stored procedure
            bool resultado = await _profesionalesPorSucursalService.EliminarRelacionAsync(id_profesional, id_sucursal);

            if (resultado)
            {
                return Json(new { success = true, message = "La relación se ha eliminado correctamente." });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo eliminar la relación." });
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CrearRelacion(int IdProfesional, int IdSucursal)
    {
        try
        {
            int nuevoId = await _profesionalesPorSucursalService.CrearRelacionAsync(IdProfesional, IdSucursal);

            if (nuevoId > 0)
            {
                return Json(new { success = true, message = "La relación se ha creado correctamente.", id = nuevoId });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo crear la relación." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
        }
    }
}
