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
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");
            return usuarioRol == "Administrador";
        }

        private bool IsUserAuthenticatedAsSuperAdmin()
        {
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");
            return usuarioRol == "SuperAdministrador";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var isSuperAdmin = IsUserAuthenticatedAsSuperAdmin();
            var isAdmin = IsUserAuthenticatedAsAdmin();

            if (isSuperAdmin)
            {
                return View("~/Views/Admin/SuperDashboard.cshtml");
            }
            else if (isAdmin)
            {
                return View("~/Views/Admin/Dashboard.cshtml");
            }
            else
            {
                return Unauthorized("No tienes permisos para ver esta información.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesData(string timeRange)
        {
            try
            {
                IEnumerable<Ventas> salesData;
                var isSuperAdmin = IsUserAuthenticatedAsSuperAdmin();
                var isAdmin = IsUserAuthenticatedAsAdmin();

                if (string.IsNullOrEmpty(timeRange))
                {
                    timeRange = "daily";
                }

                if (isSuperAdmin)
                {
                    var sucursales = await _sucursalService.GetSucursalesAsync();
                    var allSalesData = new List<object>();

                    var salesDataByBranch = await _dashboardService.GetSalesDataForAllBranches(timeRange, sucursales.Select(s => s.Id));

                    foreach (var sucursal in salesDataByBranch)
                    {
                        var salesDataForBranch = sucursal.Value.Select(row => new
                        {
                            label = row.Dia ?? row.Hora,
                            value = row.TotalVentas
                        }).ToList();

                        allSalesData.Add(new
                        {
                            sucursalName = sucursal.Key,
                            salesData = salesDataForBranch
                        });
                    }

                    return Json(allSalesData);
                }
                else if (isAdmin)
                {
                    var idString = HttpContext.Session.GetString("UsuarioId");
                    int.TryParse(idString, out var idAdmin);
                    var sucursal = await _sucursalService.ObtenerSucursalPorIdAdmin(idAdmin);

                    if (sucursal <= 0)
                    {
                        return NotFound("No se encontró la sucursal asociada al administrador.");
                    }

                    salesData = await _dashboardService.GetSalesDataForBranch(timeRange, sucursal);

                    var chartData = salesData.Select(row => new
                    {
                        label = row.Dia ?? row.Hora,
                        value = row.TotalVentas
                    });

                    return Json(chartData);
                }
                else
                {
                    return Unauthorized("No tienes permisos para ver esta información.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Hubo un problema al obtener los datos de ventas." });
            }
        }
    }
}
