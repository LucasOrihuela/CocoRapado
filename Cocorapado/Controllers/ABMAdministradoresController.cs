using Cocorapado.Service;
using Cocorapado.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");
            return usuarioRol == "SuperAdministrador";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticatedAsSuperAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var administradores = await _usuarioService.GetAdministradoresAsync();
                return View("~/Views/ABM/ABMAdministradores.cshtml", administradores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearAdministrador(Usuario nuevoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                var idRolAdministrador = await _perfilService.ObtenerIdPorRolAsync("Administrador");
                if (idRolAdministrador == null)
                {
                    return Json(new { success = false, message = "El rol 'Administrador' no fue encontrado." });
                }

                nuevoUsuario.IdPerfil = idRolAdministrador.Value;
                await _usuarioService.CrearUsuarioAsync(nuevoUsuario);

                return Json(new { success = true, message = "Administrador creado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
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
                return Json(new { success = result, message = result ? "Administrador eliminado correctamente." : "No se pudo eliminar el administrador." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioDTO administrador)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos del administrador no son válidos." });
            }

            try
            {
                var administradorExistente = await _usuarioService.ObtenerAdministradorPorIdAsync(administrador.Id);
                if (administradorExistente == null)
                {
                    return Json(new { success = false, message = "Administrador no encontrado." });
                }

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

                var result = await _usuarioService.EditarAdministradorAsync(administradorModificado);
                return Json(new { success = result, message = result ? "Administrador editado correctamente." : "No se pudo editar el administrador." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerAdministrador(int id)
        {
            try
            {
                var administrador = await _usuarioService.ObtenerAdministradorPorIdAsync(id);
                if (administrador == null)
                {
                    return Json(new { success = false, message = "Administrador no encontrado." });
                }

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
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
