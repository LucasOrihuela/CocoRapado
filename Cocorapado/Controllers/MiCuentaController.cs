using Microsoft.AspNetCore.Mvc;
using Cocorapado.Models;
using Cocorapado.Service; // Asegúrate de tener el servicio correcto

namespace Cocorapado.Controllers
{
    public class MiCuentaController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public MiCuentaController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Acción para mostrar los datos del usuario
        public async Task<IActionResult> Index()  // Agregué async y Task aquí
        {
            var usuarioEmail = HttpContext.Session.GetString("UsuarioEmail");

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Asegúrate de esperar el resultado del servicio con await
            var usuario = await _usuarioService.ObtenerUsuarioPorCorreoAsync(usuarioEmail);

            // Pasa el usuario correctamente a la vista
            return View("~/Views/Account/MiCuenta.cshtml", usuario);
        }

        // Acción asincrónica para guardar los datos editados
        [HttpPost]
        public async Task<IActionResult> EditarDatos(Usuario usuario)
        {
            var clienteId = HttpContext.Session.GetString("UsuarioId");
            if(clienteId != null) {
                usuario.Id = clienteId;
            }

            if (ModelState.IsValid)
            {
                // Llama al servicio asincrónico para editar el usuario
                bool resultado = await _usuarioService.EditarClienteAsync(usuario);

                if (resultado)
                {
                    var usuarioModificado = await _usuarioService.ObtenerUsuarioPorCorreoAsync(usuario.Correo);
                    // Redirige a la vista de la cuenta con el usuario actualizado
                    return View("~/Views/Account/MiCuenta.cshtml", usuarioModificado);
                }
                else
                {
                    // Si no se pudo actualizar, puedes mostrar un mensaje o mantener el formulario
                    ModelState.AddModelError(string.Empty, "No se pudo guardar los datos.");
                }
            }

            // Si hay errores en el modelo, vuelve a mostrar la vista con los datos ingresados
            return View("Index", usuario);
        }
    }
}
