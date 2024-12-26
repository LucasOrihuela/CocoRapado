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
    public class ABMAdministradoresController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly PerfilService _perfilService;
        private readonly SucursalService _sucursalService;

        public ABMAdministradoresController(UsuarioService usuarioService, PerfilService perfilService, SucursalService sucursalService)
        {
            _usuarioService = usuarioService;
            _perfilService = perfilService;
            _sucursalService = sucursalService;
        }

        private bool IsUserAuthenticatedAsSuperAdmin()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "SuperAdministrador";
        }

        // Acción Index para listar administradores
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticatedAsSuperAdmin())
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                IEnumerable<Usuario> administradores = Enumerable.Empty<Usuario>();
                var idString = HttpContext.Session.GetString("UsuarioId");
                int.TryParse(idString, out var idAdmin);
                var sucursal = await _sucursalService.ObtenerSucursalPorIdAdmin(idAdmin);
                administradores = await _usuarioService.GetAdministradoresAsync();

                if (administradores == null || !administradores.Any())
                {
                    return NotFound("No hay administradores registrados.");
                }

                return View("~/Views/ABM/ABMAdministradores.cshtml", administradores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Acción para crear un nuevo Administrador

        [HttpPost]
        public async Task<IActionResult> CrearAdministrador(Usuario nuevoUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var idRolAdministrador = await _perfilService.ObtenerIdPorRolAsync("Administrador");

                    if (idRolAdministrador == null)
                    {
                        return Json(new { success = false, message = "El rol 'Administrador' no fue encontrado." });
                    }

                    nuevoUsuario.Perfil = new Perfil
                    {
                        Id = idRolAdministrador.Value,
                        Rol = "Administrador"
                    };
                    nuevoUsuario.IdPerfil = idRolAdministrador.Value;

                    await _usuarioService.CrearUsuarioAsync(nuevoUsuario);
                    return Json(new { success = true, message = "Administrador creado exitosamente." });
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
                var administradorExistente = await _usuarioService.ObtenerAdministradorPorIdAsync(id);
                if (administradorExistente == null)
                {
                    return Json(new { success = false, message = "Administrador no encontrado." });
                }

                var result = await _usuarioService.EliminarAdministradorAsync(id);
                return result
                    ? Json(new { success = true, message = "El administrador se eliminó correctamente." })
                    : Json(new { success = false, message = "No se pudo eliminar el administrador." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioDTO administrador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el administrador existente de manera asincrónica
                    var administradorExistente = await _usuarioService.ObtenerAdministradorPorIdAsync(administrador.Id);

                    if (administradorExistente == null)
                    {
                        return Json(new { success = false, message = "Administrador no encontrado." });
                    }

                    // Mapear los datos del DTO hacia la entidad Usuario para editar
                    var administradorModificado = new Usuario
                    {
                        Id = administrador.Id.ToString(),
                        IdPerfil = administrador.IdPerfil,
                        Clave = administrador.Clave,
                        Correo = administrador.Correo,
                        Imagen = administrador.Imagen,
                        Nombre = administrador.Nombre,
                        Apellido = administrador.Apellido,
                        Telefono = administrador.Telefono,
                        FechaNacimiento = administrador.FechaNacimiento,
                        EstaLogueado = administrador.EstaLogueado,
                    };

                    // Editar al administrador de manera asincrónica
                    var result = await _usuarioService.EditarAdministradorAsync(administradorModificado);

                    return Json(new
                    {
                        success = result,
                        message = result ? "El administrador se editó correctamente." : "No se pudo editar el administrador."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
                }
            }

            // Retornar un error si el modelo no es válido
            return Json(new { success = false, message = "Datos del administrador no son válidos." });
        }



        [HttpGet]
        public async Task<JsonResult> ObtenerAdministrador(int id)
        {
            var administrador = await _usuarioService.ObtenerAdministradorPorIdAsync(id);

            if (administrador != null)
            {
                // Devolver el objeto en formato JSON con propiedades en minúscula
                return Json(new
                {
                    idPerfil = administrador.IdPerfil,
                    correo = administrador.Correo,
                    imagen = administrador.Imagen,
                    nombre = administrador.Nombre,
                    apellido = administrador.Apellido,
                    telefono = administrador.Telefono,
                    fechaNacimiento = administrador.FechaNacimiento,
                    estaLogueado = administrador.EstaLogueado
                });
            }

            return Json(new { success = false, message = "Administrador no encontrado." });
        }
    }
}
