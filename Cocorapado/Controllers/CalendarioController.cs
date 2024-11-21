using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using System.Threading.Tasks;
using System.Linq;
using Cocorapado.Models;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Net.Mail;
using System.Net;

public class CalendarioController : Controller
{
    private readonly SucursalService _sucursalService;
    private readonly TurnoService _turnosService;
    private readonly UsuarioService _usuarioService;
    private readonly ServicioService _servicioService;

    public CalendarioController(SucursalService sucursalService, TurnoService turnosService, UsuarioService usuarioService, ServicioService servicioService)
    {
        _sucursalService = sucursalService;
        _turnosService = turnosService;
        _usuarioService = usuarioService;
        _servicioService = servicioService;
    }

    // Método para verificar si el usuario está autenticado y tiene el rol de Cliente
    private bool IsUserAuthenticatedAsClient()
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

        return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Cliente";
    }

    public async Task<IActionResult> Index(int idSucursal, int idProfesional)
    {
        if (!IsUserAuthenticatedAsClient())
        {
            return RedirectToAction("Login", "Account");
        }

        var horarios = await _sucursalService.GetHorariosBySucursalIdAsync(idSucursal);

        if (horarios == null || !horarios.Any())
        {
            return RedirectToAction("Error", "Home");
        }

        ViewBag.Horarios = horarios;
        ViewBag.IdSucursal = idSucursal;
        ViewBag.IdProfesional = idProfesional;

        return View("Calendario");
    }


    public async Task<IActionResult> Calendario(int idSucursal, int idProfesional, string idsServicios)
    {

        // Verificar si el usuario está autenticado
        if (!IsUserAuthenticatedAsClient())
        {
            return RedirectToAction("Login", "Account"); // Redirigir al login si no está autenticado
        }

        // Convertir idsServicios a una lista de enteros
        var serviciosSeleccionados = idsServicios?.Split(',')
            .Select(int.Parse)
            .ToList();

        // Pasar los datos necesarios a la vista
        ViewBag.IdSucursal = idSucursal;
        ViewBag.IdProfesional = idProfesional;
        ViewBag.ServiciosSeleccionados = serviciosSeleccionados;

        return View("Calendario");
    }

    [HttpGet]
    public async Task<IActionResult> GetHorarios(int idSucursal)
    {
        var horarios = await _sucursalService.GetHorariosBySucursalIdAsync(idSucursal);
        return Json(horarios);
    }

    [HttpGet]
    public async Task<IActionResult> GetDuracionTurno(string idsServicios)
    {
        int duracionTotal = 0;
        var listaIdsServicios = idsServicios.Split(',')
                                            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();

        foreach (var idServicio in listaIdsServicios)
        {
            var duracionTurno = await _turnosService.GetDuracionDeServicioByIdAsync(idServicio);
            if (duracionTurno > 0)
            {
                duracionTotal += duracionTurno;
            }
        }

        return Json(duracionTotal);
    }


    [HttpGet]
    public async Task<JsonResult> GetEventos(int idSucursal, int idProfesional)
    {
        // Verificar si el usuario está autenticado
        if (!IsUserAuthenticatedAsClient())
        {
            return Json(new { error = "Unauthorized" }); // Devolver un error si no está autenticado
        }

        // Obtener turnos para los eventos del calendario
        var turnos = await _turnosService.GetTurnosPorSucursalYProfesionalAsync(idSucursal, idProfesional);

        var eventos = turnos.Select(t => new
        {
            title = "Reservado",
            start = t.Fecha.ToString("yyyy-MM-dd") + "T" + t.Hora.ToString(),
            end = t.Fecha.ToString("yyyy-MM-dd") + "T" + t.Hora.Add(TimeSpan.FromMinutes(t.DuracionMin)).ToString()
        }).ToList();

        return Json(eventos);
    }

    [HttpGet]
    public async Task<IActionResult> GetTurnoDetails(int idSucursal, int idProfesional, string idsServicios)
    {
        // Obtener el nombre de la sucursal
        var nombreSucursal = await _sucursalService.GetNombreSucursalByIdAsync(idSucursal);

        // Obtener telefono de la sucursal
        var telefonoSucursal = await _sucursalService.GetTelefonoSucursalByIdAsync(idSucursal);

        // Obtener el nombre completo del profesional
        var nombreProfesional = await _usuarioService.GetNombreProfesionalesByIdAsync(idProfesional);

        var telefonoCliente = HttpContext.Session.GetString("UsuarioTelefono");

        // Convertir `idsServicios` a una lista de enteros
        var listaIdsServicios = idsServicios.Split(',')
                                            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();

        // Lista para almacenar los nombres de los servicios
        var nombresServicios = new List<string>();

        var precioMinTotal = new List<int>();
        var precioMaxTotal = new List<int>();

        // Obtener los nombres de los servicios uno por uno
        foreach (var idServicio in listaIdsServicios)
        {

            var nombreServicio = await _servicioService.GetNombreServicioByIdAsync(idServicio);
            var preciosServicio = await _servicioService.GetPreciosServicioByIdAsync(idServicio);

            if (!string.IsNullOrEmpty(nombreServicio))
            {
                nombresServicios.Add(nombreServicio);

            }
            if (!string.IsNullOrEmpty(preciosServicio))
            {
                string[] precios = preciosServicio.Split(',');
                precioMinTotal.Add(int.Parse(precios[0]));
                precioMaxTotal.Add(int.Parse(precios[1]));
            }

        }

        // Crear el objeto de respuesta
        var resultado = new
        {
            Sucursal = nombreSucursal,
            Profesional = nombreProfesional,
            Servicios = nombresServicios,
            TelefonoSucursal = telefonoSucursal,
            telefonoCliente = telefonoCliente,
            PrecioMin = precioMinTotal,
            PrecioMax = precioMaxTotal,
        };

        return Json(resultado);
    }


    [HttpPost]
    public async Task<IActionResult> GuardarTurno(int idSucursal, int idProfesional, int duracionMin, string fecha, string hora, int precio)
    {
        try
        {
            var usuarioEmail = HttpContext.Session.GetString("UsuarioEmail");
            var usuarioId = "";
            usuarioId = HttpContext.Session.GetString("UsuarioId");
            int.TryParse(usuarioId, out var clienteId);

            // Convertir fecha y hora a un DateTime para el turno
            // Cambiando el formato a "dd/MM/yyyy HH:mm" para coincidir con la cadena de fecha
            DateTime fechaTurno = DateTime.ParseExact($"{fecha} {hora}", "dd/MM/yyyy HH:mm", null);

            // Guardar el turno en la base de datos
            int idTurno = await _turnosService.GuardarTurnoAsync(new Turno
            {
                Fecha = fechaTurno,
                Hora = TimeSpan.Parse(hora),
                IdSucursal = idSucursal,
                IdProfesional = idProfesional,
                IdCliente = clienteId,
                Precio = precio,
                DuracionMin = duracionMin
            });

            if (idTurno == 0)
            {
                return Json(new { success = false, message = "Error al guardar el turno en la base de datos." });
            }

            return Json(new { success = true, message = "Turno reservado exitosamente y email de confirmación enviado." });
        }
        catch (Exception ex)
        {
            // Log de errores
            Console.Error.WriteLine($"Error en GuardarTurno: {ex.Message}");
            return Json(new { success = false, message = "Ocurrió un error al confirmar el turno." });
        }
    }


}
