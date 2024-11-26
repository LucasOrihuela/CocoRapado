using Cocorapado.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Cocorapado.Service
{
    public class DashboardService
    {
        private readonly IDbConnection _dbConnection;

        public DashboardService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Método para obtener ventas de una sucursal específica según el rango de tiempo
        public async Task<IEnumerable<Ventas>> GetSalesDataForBranch(string timeRange, int idSucursal)
        {
            try
            {
                string procedureName = GetProcedureName(timeRange);
                var today = DateTime.Now;
                var parameters = new DynamicParameters();

                // Parámetros necesarios para el procedimiento
                parameters.Add("@id_sucursal", idSucursal);
                switch (timeRange)
                {
                    case "daily":
                        parameters.Add("@Fecha", today.Date);
                        break;
                    case "weekly":
                        parameters.Add("@FechaInicio", today.Date.AddDays(-7)); // Últimos 7 días
                        parameters.Add("@FechaFin", today.Date);
                        break;
                    case "monthly":
                        parameters.Add("@Mes", today.Month);
                        parameters.Add("@Anio", today.Year);
                        break;
                    case "annual":
                        parameters.Add("@Anio", today.Year);
                        break;
                }

                // Ejecutar el procedimiento almacenado
                return await _dbConnection.QueryAsync<Ventas>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener los datos de ventas para la sucursal.");
            }
        }

        // Método para obtener ventas de todas las sucursales (SuperAdministrador)
        public async Task<Dictionary<int, IEnumerable<Ventas>>> GetSalesDataForAllBranches(string timeRange, IEnumerable<int> sucursales)
        {
            try
            {
                var salesDataByBranch = new Dictionary<int, IEnumerable<Ventas>>();

                foreach (var idSucursal in sucursales)
                {
                    var salesData = await GetSalesDataForBranch(timeRange, idSucursal);
                    salesDataByBranch[idSucursal] = salesData;
                }

                return salesDataByBranch;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener los datos de ventas para todas las sucursales.");
            }
        }

        // Método privado para obtener el nombre del procedimiento almacenado
        private string GetProcedureName(string timeRange)
        {
            return timeRange switch
            {
                "daily" => "sp_ObtenerVentasDiariasPorSucursal",
                "weekly" => "sp_ObtenerVentasSemanalesPorSucursal",
                "monthly" => "sp_ObtenerVentasMensualesPorSucursal",
                "annual" => "sp_ObtenerVentasAnualPorSucursal",
                _ => throw new ArgumentException("El rango de tiempo especificado no es válido.")
            };
        }
    }
}
