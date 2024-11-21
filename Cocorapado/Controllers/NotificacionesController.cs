using Microsoft.AspNetCore.Mvc;

namespace Cocorapado.Controllers
{
    public class NotificacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
