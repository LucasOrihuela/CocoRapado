using Microsoft.AspNetCore.Mvc;
using Cocorapado.Service;
using Cocorapado.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Cocorapado.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UsuarioService _usuarioService;

        // Inyección de dependencias
        public AccountController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("Login")] // Ruta específica para el método Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Para redirigir después del login
            return View("~/Views/Account/Login.cshtml"); // Ruta completa a la vista Login
        }

        [HttpGet("Registro")] // Ruta específica para el método Registro
        public IActionResult Registro()
        {
            return View("~/Views/Account/Registro.cshtml"); // Ruta completa a la vista Registro
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string clave, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            {
                ModelState.AddModelError("", "El correo y la contraseña son obligatorios.");
                ViewData["ReturnUrl"] = returnUrl;
                return View("~/Views/Account/Login.cshtml");
            }

            // Comprobar si el email está registrado
            var usuario = await _usuarioService.Authenticate(email, clave);
            if (usuario == null)
            {
                ModelState.AddModelError("", "El correo electrónico no está registrado.");
                ViewData["ReturnUrl"] = returnUrl;
                return View("~/Views/Account/Login.cshtml");
            }

            // Verificar si el usuario está bloqueado
            var estaBloqueado = await _usuarioService.VerificarUsuarioBloqueado(int.Parse(usuario.Id));
            if (estaBloqueado)
            {
                ModelState.AddModelError("", "Tu cuenta está bloqueada debido a cancelaciones excesivas.");
                return View("~/Views/Account/Login.cshtml");
            }

            // Verificar la contraseña
            var passwordHasher = new PasswordHasher<Usuario>();
            var verificationResult = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, clave);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Contraseña incorrecta.");
                ViewData["ReturnUrl"] = returnUrl;
                return View("~/Views/Account/Login.cshtml");
            }

            var telefonoCliente = await _usuarioService.GetTelefonoClienteByIdAsync(usuario.Id.ToString());


            // Almacenar el usuario en la sesión
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioEmail", usuario.Correo);
            HttpContext.Session.SetString("UsuarioTelefono", telefonoCliente.ToString());
            ViewBag.UsuarioId = HttpContext.Session.GetString("UsuarioId");

            var usuarioDTO = ConvertirAUsuarioDTO(usuario);

            var idSucursal = await _usuarioService.GetSucursalByAdministradorIdAsync(usuarioDTO.Id);
            HttpContext.Session.SetString("AdministradorSucursal", idSucursal.ToString());

            // Redirigir según el rol del usuario
            var perfil = await _usuarioService.ObtenerPerfilPorUsuarioAsync(usuarioDTO.Id);
            if (perfil != null)
            {
                HttpContext.Session.SetString("UsuarioRol", perfil.Rol);

                return perfil.Rol switch
                {
                    "SuperAdministrador" => RedirectToAction("Index", "Dashboard"),
                    "Administrador" => RedirectToAction("Index", "Dashboard"),
                    "Profesional" => RedirectToAction("Index", "Profesionales"),
                    "Cliente" => RedirectToAction("Index", "Home"),
                    _ => RedirectToAction("Login", "Account")
                };
            }

            ModelState.AddModelError("", "No se encontró un perfil válido para este usuario.");
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Account/Login.cshtml");
        }


        [HttpPost("Logout")] // Ruta específica para el método Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
            return RedirectToAction("Login", "Account"); // Redirigir a la ruta de Login
        }

        [HttpPost("Registro")] // Ruta específica para el método Registro (POST)
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var result = await _usuarioService.CrearUsuarioAsync(usuario);
                if (result > 0)
                {
                    // Recuperar el usuario recién creado
                    var nuevoUsuario = await _usuarioService.ObtenerUsuarioPorCorreoAsync(usuario.Correo);
                    if (nuevoUsuario != null)
                    {
                        // Loguear automáticamente al usuario
                        HttpContext.Session.SetString("UsuarioId", nuevoUsuario.Id.ToString());
                        HttpContext.Session.SetString("UsuarioEmail", nuevoUsuario.Correo);
                        HttpContext.Session.SetString("UsuarioTelefono", nuevoUsuario.Telefono?.ToString() ?? "");

                        // Obtener el perfil del usuario y redirigir según el rol
                        var perfil = await _usuarioService.ObtenerPerfilPorUsuarioAsync(int.Parse(nuevoUsuario.Id));

                        if (perfil != null)
                        {
                            HttpContext.Session.SetString("UsuarioRol", perfil.Rol);

                            return perfil.Rol switch
                            {
                                "SuperAdministrador" => RedirectToAction("SuperDashboard", "Admin"),
                                "Administrador" => RedirectToAction("Index", "Dashboard"),
                                "Profesional" => RedirectToAction("Index", "Profesionales"),
                                "Cliente" => RedirectToAction("Index", "Home"),
                                _ => RedirectToAction("Login", "Account")
                            };
                        }
                        else
                        {
                            ModelState.AddModelError("", "El perfil del usuario no se encontró.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al recuperar los datos del usuario recién registrado.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo registrar el usuario. Intente de nuevo.");
                }
            }

            return View("~/Views/Account/Registro.cshtml", usuario);
        }


        [HttpPost]
        public async Task<IActionResult> RecuperarContraseña(string email)
        {
            // Lógica para enviar un correo de recuperación de contraseña
            if (string.IsNullOrEmpty(email))
            {
                // Manejar el caso en que el correo es nulo o vacío
                ModelState.AddModelError("", "Por favor ingrese un correo válido.");
                return View();
            }

            var usuario = await _usuarioService.ObtenerUsuarioPorCorreoAsync(email);
            if (usuario == null)
            {
                // Si el usuario no existe, manejar el error
                ModelState.AddModelError("", "El correo ingresado no está registrado.");
                return View();
            }

            // Lógica para enviar un correo de recuperación de contraseña (puedes usar un servicio de correo aquí)
            //bool correoEnviado = await _usuarioService.EnviarCorreoRecuperacion(usuario);

            //if (correoEnviado)
            //{
            //    ViewBag.Mensaje = "Te hemos enviado un correo con instrucciones para recuperar tu contraseña.";
            //    return View(); // Aquí puedes redirigir a una vista que confirme el envío del correo
            //}

            // Si algo falla
            ModelState.AddModelError("", "Hubo un error al enviar el correo de recuperación.");
            return View("~/Views/Account/RecuperarPassword.cshtml", usuario);
        }


        // Método para convertir Usuario a UsuarioDTO
        public UsuarioDTO ConvertirAUsuarioDTO(Usuario usuario)
        {
            return new UsuarioDTO
            {
                Id = int.TryParse(usuario.Id, out int id) ? id : 0,
                IdPerfil = usuario.IdPerfil,
                Correo = usuario.Correo,
                Imagen = usuario.Imagen,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono,
                EstaLogueado = usuario.EstaLogueado
            };
        }
    }
}
