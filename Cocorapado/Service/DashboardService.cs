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

        // Método para obtener las ventas generales según el rango de tiempo
        public async Task<IEnumerable<Ventas>> GetSalesData(string timeRange, int? idSucursal = null)
        {
            try
            {
                string procedureName = string.Empty;
                var today = DateTime.Now;
                var parameters = new DynamicParameters();

                // Selección de procedimiento según el rango de tiempo
                if (timeRange == "daily")
                {
                    procedureName = idSucursal.HasValue ? "sp_ObtenerVentasDiariasPorSucursal" : "sp_ObtenerVentasDiarias";
                    parameters.Add("@Fecha", today.Date);
                }
                else if (timeRange == "weekly")
                {
                    procedureName = idSucursal.HasValue ? "sp_ObtenerVentasSemanalesPorSucursal" : "sp_ObtenerVentasSemanales";
                    parameters.Add("@FechaInicio", today.Date.AddDays(-7)); // Últimos 7 días
                    parameters.Add("@FechaFin", today.Date);
                }
                else if (timeRange == "monthly")
                {
                    procedureName = idSucursal.HasValue ? "sp_ObtenerVentasMensualesPorSucursal" : "sp_ObtenerVentasMensuales";
                    parameters.Add("@Mes", today.Month);
                    parameters.Add("@Anio", today.Year);
                }
                else if (timeRange == "annual")
                {
                    procedureName = idSucursal.HasValue ? "sp_ObtenerVentasAnualPorSucursal" : "sp_ObtenerVentasAnual";
                    parameters.Add("@Anio", today.Year);
                }

                // Si tenemos un idSucursal, lo agregamos a los parámetros
                if (idSucursal.HasValue)
                {
                    parameters.Add("@id_sucursal", idSucursal.Value);
                }

                // Ejecución del procedimiento almacenado
                return await _dbConnection.QueryAsync<Ventas>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener los datos de ventas");
            }
        }

        // Método para obtener las ventas por sucursal según el rango de tiempo
        public async Task<IEnumerable<Ventas>> GetSalesDataByBranch(string timeRange, int idSucursal)
        {
            try
            {
                string procedureName = string.Empty;
                var today = DateTime.Now;
                var parameters = new DynamicParameters();
                parameters.Add("@id_sucursal", idSucursal);

                // Selección de procedimiento según el rango de tiempo
                if (timeRange == "daily")
                {
                    procedureName = "sp_ObtenerVentasDiariasPorSucursal";
                    parameters.Add("@Fecha", today.Date);
                }
                else if (timeRange == "weekly")
                {
                    procedureName = "sp_ObtenerVentasSemanalesPorSucursal";
                    parameters.Add("@FechaInicio", today.Date.AddDays(-7)); // Últimos 7 días
                    parameters.Add("@FechaFin", today.Date);
                }
                else if (timeRange == "monthly")
                {
                    procedureName = "sp_ObtenerVentasMensualesPorSucursal";
                    parameters.Add("@Mes", today.Month);
                    parameters.Add("@Anio", today.Year);
                }
                else if (timeRange == "annual")
                {
                    procedureName = "sp_ObtenerVentasAnualPorSucursal";
                    parameters.Add("@Anio", today.Year);
                }


                // Ejecución del procedimiento almacenado
                return await _dbConnection.QueryAsync<Ventas>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener los datos de ventas por sucursal");
            }
        }
    }
}
