using Cocorapado.Service;
using Cocorapado.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace Cocorapado.Controllers
{
    public class ABMProfesionalesController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly PerfilService _perfilService;
        private readonly SucursalService _sucursalService;

        public ABMProfesionalesController(UsuarioService usuarioService, PerfilService perfilService, SucursalService sucursalService)
        {
            _usuarioService = usuarioService;
            _perfilService = perfilService;
            _sucursalService = sucursalService;
        }

        private bool IsUserAuthenticatedAsClient()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
        }

        // Acción Index para listar profesionales
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticatedAsClient())
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                IEnumerable<Usuario> profesionales = Enumerable.Empty<Usuario>();
                var idString = HttpContext.Session.GetString("UsuarioId");
                int.TryParse(idString, out var idAdmin);
                var sucursal = await _sucursalService.ObtenerSucursalPorIdAdmin(idAdmin);
                if (sucursal > 0)
                {
                    profesionales = await _usuarioService.GetProfesionalesBySucursalIdAsync(sucursal);
                }

                if (profesionales == null || !profesionales.Any())
                {
                    return NotFound("No hay profesionales registrados.");
                }

                return View("~/Views/ABM/ABMProfesionales.cshtml", profesionales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Acción para crear un nuevo profesional

        [HttpPost]
        public async Task<IActionResult> CrearProfesional(Usuario nuevoUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var idRolProfesional = await _perfilService.ObtenerIdPorRolAsync("Profesional");

                    if (idRolProfesional == null)
                    {
                        return Json(new { success = false, message = "El rol 'Profesional' no fue encontrado." });
                    }

                    nuevoUsuario.Perfil = new Perfil
                    {
                        Id = idRolProfesional.Value,
                        Rol = "Profesional"
                    };
                    nuevoUsuario.IdPerfil = idRolProfesional.Value;

                    await _usuarioService.CrearUsuarioAsync(nuevoUsuario);
                    return Json(new { success = true, message = "Profesional creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Datos inválidos." });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var profesionalExistente = await _usuarioService.ObtenerProfesionalPorIdAsync(id);
                if (profesionalExistente == null)
                {
                    return Json(new { success = false, message = "Profesional no encontrado." });
                }

                var result = await _usuarioService.EliminarProfesionalAsync(id);
                return result
                    ? Json(new { success = true, message = "El profesional se eliminó correctamente." })
                    : Json(new { success = false, message = "No se pudo eliminar el profesional." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioDTO profesional)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el profesional existente de manera asincrónica
                    var profesionalExistente = await _usuarioService.ObtenerProfesionalPorIdAsync(profesional.Id);

                    if (profesionalExistente == null)
                    {
                        return Json(new { success = false, message = "Profesional no encontrado." });
                    }

                    // Mapear los datos del DTO hacia la entidad Usuario para editar
                    var profesionalModificado = new Usuario
                    {
                        Id = profesional.Id.ToString(),
                        IdPerfil = profesional.IdPerfil,
                        Clave = profesional.Clave,
                        Correo = profesional.Correo,
                        Imagen = profesional.Imagen,
                        Nombre = profesional.Nombre,
                        Apellido = profesional.Apellido,
                        Telefono = profesional.Telefono,
                        FechaNacimiento = profesional.FechaNacimiento,
                        EstaLogueado = profesional.EstaLogueado,
                    };

                    // Editar al profesional de manera asincrónica
                    var result = await _usuarioService.EditarProfesionalAsync(profesionalModificado);

                    return Json(new
                    {
                        success = result,
                        message = result ? "El profesional se editó correctamente." : "No se pudo editar el profesional."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
                }
            }

            // Retornar un error si el modelo no es válido
            return Json(new { success = false, message = "Datos del profesional no son válidos." });
        }



        [HttpGet]
        public async Task<JsonResult> ObtenerProfesional(int id)
        {
            var profesional = await _usuarioService.ObtenerProfesionalPorIdAsync(id);

            if (profesional != null)
            {
                // Devolver el objeto en formato JSON con propiedades en minúscula
                return Json(new
                {
                    idPerfil = profesional.IdPerfil,
                    correo = profesional.Correo,
                    imagen = profesional.Imagen,
                    nombre = profesional.Nombre,
                    apellido = profesional.Apellido,
                    telefono = profesional.Telefono,
                    fechaNacimiento = profesional.FechaNacimiento,
                    estaLogueado = profesional.EstaLogueado
                });
            }

            return Json(new { success = false, message = "Profesional no encontrado." });
        }
    }
}
