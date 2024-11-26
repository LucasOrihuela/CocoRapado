using Microsoft.AspNetCore.Mvc;
using Cocorapado.Models;
using Cocorapado.Service;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cocorapado.Controllers
{
    public class ABMSucursalesController : Controller
    {
        private readonly SucursalService _sucursalService;

        public ABMSucursalesController(SucursalService sucursalService)
        {
            _sucursalService = sucursalService;
        }

        private bool IsUserAuthenticatedAsSuperAdmin()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "SuperAdministrador";
        }

        // GET: /ABM/ABMSucursales
        public async Task<IActionResult> Index()
        {

            if (!IsUserAuthenticatedAsSuperAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtén la lista de sucursales desde el servicio
            var sucursales = await _sucursalService.GetSucursalesAsync();

            // Mapea la lista de Sucursal a SucursalDTO
            var sucursalesDTO = sucursales.Select(s => new SucursalDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Imagen = s.Imagen,
                Telefono = s.Telefono,
                PrecioAbono = s.PrecioAbono
            }).ToList();

            // Retorna la lista de SucursalDTO a la vista
            return View("~/Views/ABM/ABMSucursales.cshtml", sucursalesDTO);
        }


        //// GET: /ABM/ABMSucursales/Details/5
        //public async Task<IActionResult> Details(int id)
        //{
        //    var sucursal = await _sucursalService.GetSucursalByIdAsync(id);
        //    if (sucursal == null)
        //    {
        //        return NotFound();
        //    }

        //    // Obtener horarios y servicios para la sucursal
        //    var horarios = await _sucursalService.GetHorariosBySucursalIdAsync(id);
        //    var servicios = await _sucursalService.GetServiciosBySucursalIdAsync(id);
        //    if (servicios != null)
        //    {
        //        sucursal.Servicios = servicios.Where(s => s.Checked == true).ToList();
        //    }

        //    // Pasar la sucursal, horarios, servicios y servicios seleccionados a la vista
        //    var viewModel = new SucursalDetailsViewModel
        //    {
        //        Sucursal = sucursal,
        //        Horarios = horarios,
        //        Servicios = servicios,
        //    };

        //    return View("~/Views/ABM/Details.cshtml", viewModel);
        //}

        // Acción para obtener los horarios de una sucursal
        [HttpGet]
        public async Task<JsonResult> ObtenerHorarios(int id)
        {
            var horarios = await _sucursalService.GetHorariosBySucursalIdAsync(id);

            if (horarios != null)
            {
                return Json(new { success = true, data = horarios });
            }

            return Json(new { success = false, message = "No se encontraron horarios para esta sucursal." });
        }

        // Acción para obtener los servicios de una sucursal
        [HttpGet]
        public async Task<JsonResult> ObtenerServicios(int id)
        {
            var servicios = await _sucursalService.GetServiciosBySucursalIdAsync(id);

            if (servicios != null)
            {
                return Json(new { success = true, data = servicios });
            }

            return Json(new { success = false, message = "No se encontraron servicios." });
        }

        // Acción para obtener una sucursal por ID 
        [HttpGet]
        public async Task<JsonResult> ObtenerSucursal(int id)
        {
            var sucursal = await _sucursalService.GetSucursalByIdAsync(id);

            if (sucursal != null)
            {
                return Json(new
                {
                    id = sucursal.Id,
                    nombre = sucursal.Nombre,
                    direccion = sucursal.Direccion,
                    localidad = sucursal.Localidad,
                    telefono = sucursal.Telefono,
                    precioAbono = sucursal.PrecioAbono,
                    imagen = sucursal.Imagen
                });
            }

            return Json(new { success = false, message = "Sucursal no encontrada." });
        }

        // POST: /ABM/ABMSucursales/EditarSucursal
        [HttpPost]
        public async Task<IActionResult> EditarSucursal(SucursalDTO sucursalDTO, IFormFile? Imagen, string horarios, string servicios)
        {
            if (ModelState.IsValid)
            {
                // Validar que horarios y servicios no estén vacíos
                if (string.IsNullOrWhiteSpace(horarios) || string.IsNullOrWhiteSpace(servicios))
                {
                    return Json(new { success = false, message = "Horarios y servicios no pueden estar vacíos." });
                }

                try
                {
                    // Deserializar los datos JSON
                    var horariosDict = JsonConvert.DeserializeObject<Dictionary<string, Horario>>(horarios);
                    var serviciosSeleccionados = JsonConvert.DeserializeObject<List<int>>(servicios);

                    // Validar que el diccionario de horarios no esté vacío
                    if (horariosDict == null || !horariosDict.Any())
                    {
                        return Json(new { success = false, message = "No se recibieron horarios válidos." });
                    }

                    // Crear la instancia de Sucursal
                    Sucursal sucursal = new Sucursal
                    {
                        Id = sucursalDTO.Id,
                        Nombre = sucursalDTO.Nombre,
                        Direccion = sucursalDTO.Direccion,
                        Localidad = sucursalDTO.Localidad,
                        Imagen = sucursalDTO.Imagen,
                        Telefono = sucursalDTO.Telefono
                    };

                    var carpetaDestino = "wwwroot/assets/img/sucursales";
                    var nombreImagen = sucursal.Nombre.Replace(" ", "_") + Path.GetExtension(Imagen?.FileName ?? string.Empty);
                    var rutaImagen = Path.Combine(carpetaDestino, nombreImagen);

                    // Manejar la imagen
                    if (Imagen != null && Imagen.Length > 0)
                    {
                        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(Imagen.FileName).ToLowerInvariant();

                        if (!validExtensions.Contains(extension))
                        {
                            return Json(new { success = false, message = "El archivo debe ser una imagen (JPG, PNG, GIF)." });
                        }

                        // Eliminar la imagen anterior si existe
                        if (!string.IsNullOrEmpty(sucursal.Imagen) && System.IO.File.Exists(Path.Combine("wwwroot", sucursal.Imagen)))
                        {
                            System.IO.File.Delete(Path.Combine("wwwroot", sucursal.Imagen));
                        }

                        // Guardar la nueva imagen
                        using (var stream = new FileStream(rutaImagen, FileMode.Create))
                        {
                            await Imagen.CopyToAsync(stream);
                        }

                        sucursal.Imagen = $"/assets/img/sucursales/{nombreImagen}";
                    }

                    // Actualizar la sucursal
                    await _sucursalService.UpdateSucursalAsync(sucursal);

                    // Actualizar los horarios en la base de datos
                    await _sucursalService.UpdateHorariosAsync(sucursal.Id, horariosDict);

                    // Actualizar los servicios en la base de datos
                    await _sucursalService.UpdateServiciosSucursalAsync(sucursal.Id, serviciosSeleccionados);

                    return Json(new { success = true, message = "Sucursal actualizada correctamente." });
                }
                catch (JsonException ex)
                {
                    return Json(new { success = false, message = $"Error al procesar datos JSON: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Error al actualizar la sucursal. Verifique los datos." });
        }

        [HttpPost]
        public async Task<IActionResult> CrearSucursal(SucursalDTO sucursalDTO, IFormFile? Imagen, string horarios, string servicios)
        {
            if (ModelState.IsValid)
            {
                // Validar que horarios y servicios no estén vacíos
                if (string.IsNullOrWhiteSpace(horarios) || string.IsNullOrWhiteSpace(servicios))
                {
                    return Json(new { success = false, message = "Horarios y servicios no pueden estar vacíos." });
                }

                try
                {
                    // Deserializar los datos JSON
                    var serviciosSeleccionados = new List<int> { 0 };
                    var horariosDict = JsonConvert.DeserializeObject<Dictionary<string, Horario>>(horarios);
                    serviciosSeleccionados = JsonConvert.DeserializeObject<List<int>>(servicios);
                    // Validar que el diccionario de horarios no esté vacío
                    if (horariosDict == null || !horariosDict.Any())
                    {
                        return Json(new { success = false, message = "No se recibieron horarios válidos." });
                    }

                    // Crear una nueva instancia de Sucursal
                    Sucursal sucursal = new Sucursal
                    {
                        Nombre = sucursalDTO.Nombre,
                        Direccion = sucursalDTO.Direccion,
                        Localidad = sucursalDTO.Localidad,
                        Telefono = sucursalDTO.Telefono
                    };

                    var carpetaDestino = "wwwroot/assets/img/sucursales";
                    var nombreImagen = sucursal.Nombre.Replace(" ", "_") + Path.GetExtension(Imagen?.FileName ?? string.Empty);
                    var rutaImagen = Path.Combine(carpetaDestino, nombreImagen);

                    // Manejar la imagen
                    if (Imagen != null && Imagen.Length > 0)
                    {
                        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(Imagen.FileName).ToLowerInvariant();

                        if (!validExtensions.Contains(extension))
                        {
                            return Json(new { success = false, message = "El archivo debe ser una imagen (JPG, PNG, GIF)." });
                        }

                        // Guardar la nueva imagen
                        using (var stream = new FileStream(rutaImagen, FileMode.Create))
                        {
                            await Imagen.CopyToAsync(stream);
                        }

                        sucursal.Imagen = $"/assets/img/sucursales/{nombreImagen}";
                    }

                    // Guardar la nueva sucursal en la base de datos
                    int idSucursal = await _sucursalService.CrearSucursalAsync(sucursal);

                    // Guardar los horarios en la base de datos
                    await _sucursalService.CrearHorarioAsync(idSucursal, horariosDict);

                    // Guardar los servicios en la base de datos
                    if (serviciosSeleccionados != null)
                    {
                        foreach (var idServicio in serviciosSeleccionados)
                        {
                            await _sucursalService.CrearServicioPorSucursalAsync(idServicio, idSucursal);
                        }
                    }                 

                    return Json(new { success = true, message = "Sucursal creada correctamente." });
                }
                catch (JsonException ex)
                {
                    return Json(new { success = false, message = $"Error al procesar datos JSON: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Error al crear la sucursal. Verifique los datos." });
        }



        // POST: /ABM/ABMSucursales/EliminarSucursal
        [HttpPost]
        public async Task<IActionResult> EliminarSucursal(int id)
        {
            var sucursal = await _sucursalService.GetSucursalByIdAsync(id);
            if (sucursal == null)
            {
                return Json(new { success = false, message = "No se encontró la sucursal." });
            }

            // Eliminar horarios y servicios asociados
            await _sucursalService.EliminarHorariosPorSucursalAsync(id);
            await _sucursalService.EliminarServicioPorSucursalAsync(id);

            // Eliminar la imagen si existe
            if (!string.IsNullOrEmpty(sucursal.Imagen))
            {
                // Normalizar la ruta eliminando caracteres iniciales "/"
                string rutaImagenRelativa = sucursal.Imagen.TrimStart('/');
                string rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rutaImagenRelativa);

                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }
            }

            // Eliminar la sucursal
            await _sucursalService.DeleteSucursalAsync(id);

            return Json(new { success = true, message = "Sucursal eliminada correctamente." });
        }



        // Nuevo método para actualizar los servicios seleccionados
        [HttpPost]
        public async Task<IActionResult> ActualizarServiciosSucursal(int idSucursal, List<int> serviciosSeleccionados)
        {
            // Eliminar todas las relaciones de servicios actuales para esta sucursal
                await _sucursalService.EliminarServicioPorSucursalAsync(idSucursal);

            // Agregar las nuevas relaciones de servicios seleccionados
            foreach (var idServicio in serviciosSeleccionados)
            {
                await _sucursalService.CrearServicioPorSucursalAsync(idServicio, idSucursal);
            }

            return Json(new { success = true, message = "Servicios actualizados correctamente." });
        }

    }
}
