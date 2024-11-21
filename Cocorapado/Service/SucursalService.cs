namespace Cocorapado.Service
{
    // Services/SucursalService.cs
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using Cocorapado.Models;
    using System.Data;

    public class SucursalService
    {
        private readonly string _connectionString;

        public SucursalService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra en el archivo de configuración.");
        }

        public async Task<IEnumerable<Sucursal>> GetSucursalesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerTodasLasSucursales";

                return await connection.QueryAsync<Sucursal>(storedProcedure, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Sucursal?> GetSucursalByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerSucursalPorId";

                return await connection.QueryFirstOrDefaultAsync<Sucursal>(storedProcedure, new { SucursalId = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<String?> GetNombreSucursalByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerNombreSucursalPorId";

                return await connection.QueryFirstOrDefaultAsync<string>(storedProcedure, new { SucursalId = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<String?> GetTelefonoSucursalByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerTelefonoSucursalPorId";

                return await connection.QueryFirstOrDefaultAsync<string>(storedProcedure, new { SucursalId = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> CrearSucursalAsync(Sucursal sucursal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_CrearSucursal";

                // Aquí estamos esperando el valor devuelto de SCOPE_IDENTITY()
                int idSucursal = await connection.QuerySingleAsync<int>(
                    storedProcedure,
                    new
                    {
                        sucursal.Nombre,
                        sucursal.Direccion,
                        sucursal.Localidad,
                        sucursal.Imagen,
                        sucursal.Telefono,
                        sucursal.PrecioAbono
                    },
                    commandType: CommandType.StoredProcedure);

                return idSucursal; // Devuelve el ID de la sucursal recién creada
            }
        }


        // Método para crear un servicio para una sucursal
        public async Task CrearServicioPorSucursalAsync(int idServicio, int idSucursal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(
                    "sp_CrearServicio_x_Sucursal",
                    new { id_servicio = idServicio, id_sucursal = idSucursal },
                    commandType: CommandType.StoredProcedure);
            }
        }


        public async Task CrearHorarioAsync(int idSucursal, Dictionary<string, Horario> horariosDict)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_CrearHorario";

                foreach (var horario in horariosDict)
                {
                    await connection.ExecuteAsync(
                        storedProcedure,
                        new
                        {
                            IdSucursal = idSucursal,
                            Dia = horario.Key,
                            HorarioApertura = horario.Value.HorarioApertura,
                            HorarioCierreMediodia = horario.Value.HorarioCierreMediodia,
                            HorarioAperturaMediodia = horario.Value.HorarioAperturaMediodia,
                            HorarioCierre = horario.Value.HorarioCierre
                        },
                        commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task UpdateSucursalAsync(Sucursal sucursal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ActualizarSucursal";
                var parameters = new
                {
                    Id = sucursal.Id,
                    Nombre = sucursal.Nombre,
                    Direccion = sucursal.Direccion,
                    Localidad = sucursal.Localidad,
                    Imagen = sucursal.Imagen,
                    Telefono = sucursal.Telefono,
                    PrecioAbono = sucursal.PrecioAbono
                };

                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateHorariosAsync(int idSucursal, Dictionary<string, Horario> horarios)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                foreach (var kvp in horarios)
                {
                    var dia = kvp.Key;  // Clave del diccionario (el día de la semana)
                    var horario = kvp.Value;  // Valor del diccionario (el objeto Horario)

                    var parameters = new
                    {
                        IdSucursal = idSucursal,
                        Dia = dia,
                        HorarioApertura = horario.HorarioApertura,
                        HorarioCierreMediodia = horario.HorarioCierreMediodia,
                        HorarioAperturaMediodia = horario.HorarioAperturaMediodia,
                        HorarioCierre = horario.HorarioCierre
                    };

                    await connection.ExecuteAsync("sp_ActualizarHorario", parameters, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task UpdateServiciosSucursalAsync(int idSucursal, List<int> serviciosSeleccionados)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Primero, eliminar las relaciones existentes utilizando el stored procedure
                await connection.ExecuteAsync("sp_EliminarServicio_x_Sucursal", new { id_sucursal = idSucursal }, commandType: CommandType.StoredProcedure);

                // Luego, insertar las nuevas relaciones utilizando el stored procedure
                foreach (var idServicio in serviciosSeleccionados)
                {
                    var parameters = new
                    {
                        id_servicio = idServicio,
                        id_sucursal = idSucursal
                    };

                    await connection.ExecuteAsync("sp_CrearServicioPorSucursal", parameters, commandType: CommandType.StoredProcedure);
                }
            }
        }


        public async Task DeleteSucursalAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_EliminarSucursal";
                await connection.ExecuteAsync(storedProcedure, new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> SucursalExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_SucursalExiste";
                return await connection.ExecuteScalarAsync<int>(storedProcedure, new { Id = id }, commandType: CommandType.StoredProcedure) > 0;
            }
        }

        public async Task<IEnumerable<Horario>> GetHorariosBySucursalIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerHorariosPorSucursal";
                return await connection.QueryAsync<Horario>(
                    storedProcedure,
                    new { IdSucursal = id },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<IEnumerable<Servicio>> GetServiciosBySucursalIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerServiciosPorSucursal";
                return await connection.QueryAsync<Servicio>(
                    storedProcedure,
                    new { IdSucursal = id },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<IEnumerable<SucursalDTO>> ObtenerSucursalesSinAsignarAsync(int idProfesional)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sp = "sp_ObtenerSucursalesPorProfesionalSinAsignar";
                var sucursales = await connection.QueryAsync<SucursalDTO>(
                    sp,
                    new {IdProfesional = idProfesional },
                    commandType: CommandType.StoredProcedure
                );
                return sucursales;
            }
        }

        public async Task EliminarServicioPorSucursalAsync(int idSucursal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("sp_EliminarServicio_x_Sucursal", new { id_sucursal = idSucursal }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task EliminarHorariosPorSucursalAsync(int idSucursal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("sp_EliminarHorariosPorSucursal", new { id_sucursal = idSucursal }, commandType: CommandType.StoredProcedure);
            }
        }
    }
    
}
