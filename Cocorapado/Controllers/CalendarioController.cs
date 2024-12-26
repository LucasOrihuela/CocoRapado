using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using System.Threading.Tasks;
using System.Linq;
using Cocorapado.Models;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using Google;
using Microsoft.Extensions.Configuration;

public class CalendarioController : Controller
{
    private readonly SucursalService _sucursalService;
    private readonly TurnoService _turnosService;
    private readonly UsuarioService _usuarioService;
    private readonly ServicioService _servicioService;
    private readonly IConfiguration _configuration;
    private readonly EmailSender _emailSender; // Inyectar EmailSender

    // Constructor actualizado para inyectar EmailSender
    public CalendarioController(SucursalService sucursalService, TurnoService turnosService, UsuarioService usuarioService, ServicioService servicioService, IConfiguration configuration, EmailSender emailSender)
    {
        _sucursalService = sucursalService;
        _turnosService = turnosService;
        _usuarioService = usuarioService;
        _servicioService = servicioService;
        _configuration = configuration;
        _emailSender = emailSender; // Asignar EmailSender al campo
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
        if (!IsUserAuthenticatedAsClient())
        {
            return RedirectToAction("Login", "Account");
        }

        var serviciosSeleccionados = idsServicios?.Split(',')
            .Select(int.Parse)
            .ToList();

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
        if (!IsUserAuthenticatedAsClient())
        {
            return Json(new { error = "Unauthorized" });
        }

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
        var nombreSucursal = await _sucursalService.GetNombreSucursalByIdAsync(idSucursal);
        var telefonoSucursal = await _sucursalService.GetTelefonoSucursalByIdAsync(idSucursal);
        var nombreProfesional = await _usuarioService.GetNombreProfesionalesByIdAsync(idProfesional);
        var telefonoCliente = HttpContext.Session.GetString("UsuarioTelefono");

        var listaIdsServicios = idsServicios.Split(',')
                                            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();

        var nombresServicios = new List<string>();
        var precioMinTotal = new List<int>();
        var precioMaxTotal = new List<int>();

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
            var usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                return Json(new { success = false, message = "No se encontró el correo del usuario." });
            }

            int.TryParse(usuarioId, out var clienteId);

            if (!DateTime.TryParseExact($"{fecha} {hora}", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime fechaTurno))
            {
                return Json(new { success = false, message = "El formato de fecha y hora es incorrecto." });
            }

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

            string asunto = "Confirmación de tu turno";
            string cuerpo = $@"
            Hola,
            Tu turno ha sido reservado exitosamente.
            Detalles del turno:
            - Fecha: {fecha}
            - Hora: {hora}
            - Duración: {duracionMin} minutos
            - Código de turno: {idTurno}

            ¡Gracias por elegirnos!
            ";

            try
            {
                // Usar la instancia inyectada de EmailSender
                await _emailSender.SendEmailAsync(usuarioEmail, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al enviar email: {ex.Message}");
                return Json(new { success = false, message = "Ocurrió un error al enviar el email de confirmación." });
            }

            return Json(new { success = true, message = "Turno reservado exitosamente y email de confirmación enviado." });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error en GuardarTurno: {ex.Message}\n{ex.StackTrace}");
            return Json(new { success = false, message = "Ocurrió un error al confirmar el turno." });
        }
    }
}
