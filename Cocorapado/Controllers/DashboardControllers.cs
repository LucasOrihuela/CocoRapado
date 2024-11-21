using Cocorapado.Service;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Cocorapado.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public DashboardController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private bool IsUserAuthenticatedAsClient()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            return !string.IsNullOrEmpty(usuarioId) && usuarioRol == "Administrador";
        }

        // Acción para mostrar la vista del Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            if (!IsUserAuthenticatedAsClient())
            {
                return RedirectToAction("Login", "Account");
            }
            // Especifica la carpeta "Admin" donde se encuentra la vista
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        // Acción para obtener los datos de ventas en un rango de tiempo
        [HttpGet]
        public async Task<IActionResult> GetSalesData(string timeRange)
        {
            try
            {
                string procedureName = string.Empty;
                var today = DateTime.Now;
                var parameters = new DynamicParameters();

                if (timeRange == "daily")
                {
                    procedureName = "sp_ObtenerVentasDiarias";
                    parameters.Add("@Fecha", today.Date);
                }
                else if (timeRange == "weekly")
                {
                    procedureName = "sp_ObtenerVentasSemanales";
                    parameters.Add("@FechaInicio", today.Date.AddDays(-56));
                    parameters.Add("@FechaFin", today.Date);
                }
                else if (timeRange == "monthly")
                {
                    procedureName = "sp_ObtenerVentasMensuales";
                    parameters.Add("@Mes", today.Month);
                    parameters.Add("@Anio", today.Year);
                }

                // Llamar al procedimiento almacenado
                var salesData = await _dbConnection.QueryAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                // Convertir los datos a un formato adecuado para el gráfico
                var chartData = salesData.Select(row => new
                {
                    label = row.Dia ?? row.Hora,
                    value = row.TotalVentas
                });

                return Json(chartData);
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Hubo un problema al obtener los datos de ventas" });
            }
        }


    }
}
