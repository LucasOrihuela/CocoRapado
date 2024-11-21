using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using Cocorapado.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ABMServiciosPorProfesionalController : Controller
{
    private readonly ILogger<ABMServiciosPorProfesionalController> _logger;
    private readonly SucursalService _sucursalService;
    private readonly ServiciosPorProfesionalService _serviciosPorProfesionalService;
    private readonly UsuarioService _usuarioService;
    private readonly ServicioService _servicioService;

    public ABMServiciosPorProfesionalController(ILogger<ABMServiciosPorProfesionalController> logger, ServiciosPorProfesionalService serviciosPorProfesionalService, SucursalService sucursalService, UsuarioService usuarioService, ServicioService servicioService)
    {
        _logger = logger;
        _serviciosPorProfesionalService = serviciosPorProfesionalService;
        _sucursalService = sucursalService;
        _usuarioService = usuarioService;
        _servicioService = servicioService;
    }

    private bool IsUserAuthenticatedAsClient()
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

        return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
    }

    // Acción para la vista de servicios por profesional
    public async Task<IActionResult> Index(int? idSucursal)
    {

        if (!IsUserAuthenticatedAsClient())
        {
            return RedirectToAction("Login", "Account");
        }
        // Verifica que los servicios no sean null antes de utilizarlos
        if (_serviciosPorProfesionalService == null || _sucursalService == null)
        {
            return StatusCode(500, "Error en la configuración de los servicios.");
        }

        // Obtener relaciones de servicios por profesional utilizando el servicio
        var relaciones = await _serviciosPorProfesionalService.ObtenerRelacionesPorSucursalAsync(idSucursal);

        // Maneja el caso en que no haya relaciones
        if (relaciones == null || relaciones.Count == 0)
        {
            relaciones = new List<ServiciosPorProfesionalDTO>();
        }

        // Obtener lista de sucursales para el dropdown
        var sucursales = await _sucursalService.GetSucursalesAsync();
        ViewBag.Sucursales = sucursales;

        // Retorna la vista con las relaciones
        return View("~/Views/ABM/ABMServiciosPorProfesional.cshtml", relaciones);
    }

    public async Task<IActionResult> ObtenerProfesionalesYServicios()
    {
        // Obtener la lista de profesionales y proyectar solo el nombre y apellido
        var profesionales = await _usuarioService.GetTodosLosProfesionalesAsync();
        var profesionalesFiltrados = profesionales.Select(p => new
        {
            p.Id,
            p.Nombre,
            p.Apellido
        }).ToList();

        // Obtener la lista de servicios y proyectar solo el nombre del servicio
        var servicios = await _servicioService.GetTodosLosServiciosAsync();
        var serviciosFiltrados = servicios.Select(s => new
        {
            s.Id,
            s.ServicioNombre
        }).ToList();

        // Devolver los datos como JSON
        return Json(new { profesionales = profesionalesFiltrados, servicios = serviciosFiltrados });
    }

    // Acción para obtener servicios sin asignar para un profesional
    [HttpPost]
    public async Task<IActionResult> ObtenerServiciosSinAsignar(int id_profesional)
    {
        try
        {
            // Llamar al método del servicio que obtiene servicios sin asignar
            var serviciosSinAsignar = await _servicioService.ObtenerServiciosSinAsignarAsync(id_profesional);

            // Verifica si se obtuvieron servicios
            if (serviciosSinAsignar == null || !serviciosSinAsignar.Any())
            {
                return Json(new { success = false, message = "No se encontraron servicios para este profesional." });
            }

            // Proyectar los resultados a un formato más simple
            var serviciosFiltrados = serviciosSinAsignar.Select(s => new
            {
                id = s.Id,
                servicio = s.ServicioNombre
            }).ToList();

            return Json(new { success = true, servicios = serviciosFiltrados });
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return Json(new { success = false, message = "Ocurrió un error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EliminarRelacion(int id_profesional, int id_servicio)
    {
        try
        {
            // Llamar al método del servicio que ejecuta el stored procedure
            bool resultado = await _serviciosPorProfesionalService.EliminarRelacionAsync(id_profesional, id_servicio);

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
    public async Task<IActionResult> CrearRelacion(int IdProfesional, int IdServicio)
    {
        try
        {
            int nuevoId = await _serviciosPorProfesionalService.CrearRelacionAsync(IdProfesional, IdServicio);

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
