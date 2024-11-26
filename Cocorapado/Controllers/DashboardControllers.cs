using Cocorapado.Models;
using Cocorapado.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cocorapado.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly SucursalService _sucursalService;

        public DashboardController(DashboardService dashboardService, SucursalService sucursalService)
        {
            _dashboardService = dashboardService;
            _sucursalService = sucursalService;
        }

        private bool IsUserAuthenticatedAsAdmin()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
        }

        private bool IsUserAuthenticatedAsSuperAdmin()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "SuperAdministrador";
        }

        // Acción para mostrar la vista del Dashboard de ventas generales (solo para Administradores)
        [HttpGet]
        public IActionResult Index()
        {
            if (!IsUserAuthenticatedAsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }
            // Especifica la carpeta "Admin" donde se encuentra la vista
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        // Acción para mostrar la vista de SuperDashboard de ventas por sucursal (solo para SuperAdministradores)
        [HttpGet]
        public async Task<IActionResult> SuperDashboard()
        {
            if (!IsUserAuthenticatedAsSuperAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener las sucursales
            var sucursales = await _sucursalService.GetSucursalesAsync();

            // Pasar las sucursales a la vista para que puedas mostrar las ventas por sucursal
            return View("~/Views/Admin/SuperDashboard.cshtml", sucursales);
        }

        // Acción para obtener los datos de ventas generales en un rango de tiempo
        [HttpGet]
        public async Task<IActionResult> GetSalesData(string timeRange)
        {
            try
            {
                // Llamar al servicio para obtener las ventas generales sin idSucursal
                var salesData = await _dashboardService.GetSalesData(timeRange);

                // Convertir los datos a un formato adecuado para el gráfico
                var chartData = salesData.Select(row => new
                {
                    label = row.Dia ?? row.Hora, // Usa las propiedades correctas
                    value = row.TotalVentas
                });

                return Json(chartData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Hubo un problema al obtener los datos de ventas" });
            }
        }

        // Acción para obtener los datos de ventas por sucursal en un rango de tiempo
        [HttpGet]
        public async Task<IActionResult> GetSalesDataByBranch(string timeRange)
        {
            try
            {
                // Obtener todas las sucursales
                var sucursales = await _sucursalService.GetSucursalesAsync();
                var allSalesData = new List<object>();

                foreach (var sucursal in sucursales)
                {
                    // Llamar al servicio con idSucursal para obtener las ventas por sucursal
                    var salesData = await _dashboardService.GetSalesData(timeRange, sucursal.Id);

                    // Adaptar las propiedades del modelo de ventas para el gráfico
                    var salesDataForBranch = salesData.Select(row => new
                    {
                        label = row.Dia ?? row.Hora, // Usar 'Dia' o 'Hora' dependiendo del tipo de venta
                        value = row.TotalVentas
                    }).ToList();

                    allSalesData.Add(new
                    {
                        sucursalName = sucursal.Nombre,
                        salesData = salesDataForBranch
                    });
                }

                return Json(allSalesData); // Devolver todos los datos de ventas por sucursal
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Hubo un problema al obtener los datos de ventas" });
            }
        }
    }
}
