using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Cocorapado.Models;
using System.Data;
using System.Collections.Immutable;

namespace Cocorapado.Service
{
    public class TurnoService
    {
        private readonly string _connectionString;

        public TurnoService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encuentra en el archivo de configuración.");
        }


        // Método para obtener todos los turnos por sucursal y profesional
        public async Task<IEnumerable<Turno>> GetTurnosPorSucursalYProfesionalAsync(int idSucursal, int idProfesional)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string spTurnos = "sp_ObtenerTurnosPorSucursal";

                // Obtener los turnos
                var turnos = await connection.QueryAsync<Turno>(
                    spTurnos,
                    new { IdSucursal = idSucursal, IdProfesional = idProfesional },
                    commandType: CommandType.StoredProcedure
                );

                return turnos;
            }
        }

        public async Task<IEnumerable<Turno>> GetTurnosPorClienteAsync(int idCliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string spTurnos = "sp_ObtenerTurnosPorCliente";

                // Obtener los turnos
                var turnos = await connection.QueryAsync<Turno>(
                    spTurnos,
                    new { IdCliente = idCliente },
                    commandType: CommandType.StoredProcedure
                );

                return turnos;
            }
        }

        public async Task<IEnumerable<Turno>> GetTurnosPorProfesionalAsync(int idProfesional)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string spTurnos = "sp_ObtenerTurnosPorProfesional";

                // Obtener los turnos
                var turnos = await connection.QueryAsync<Turno>(
                    spTurnos,
                    new { IdProfesional = idProfesional },
                    commandType: CommandType.StoredProcedure
                );

                return turnos;
            }
        }

        public async Task<int> GetDuracionDeServicioByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_ObtenerDuracionServicioPorId";

                return await connection.QueryFirstOrDefaultAsync<int>(storedProcedure, new { IdServicio = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> MarcarClienteAusente(int idTurno)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_MarcarClienteAusente";

                var result = await connection.QueryFirstOrDefaultAsync<int>(storedProcedure, new { id_turno = idTurno }, commandType: CommandType.StoredProcedure);

                return result > 0;
            }
        }

        public async Task<int> GuardarTurnoAsync(Turno turno)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_CrearTurno";

                var parameters = new
                {
                    fecha = turno.Fecha.Date,
                    hora = turno.Hora,
                    idSucursal = turno.IdSucursal,
                    idProfesional = turno.IdProfesional,
                    idCLiente = turno.IdCliente,
                    precio = turno.Precio,
                    duracion_min = turno.DuracionMin
                };

                connection.Open();

                // Ejecuta el procedimiento almacenado y obtiene el ID del turno insertado
                var idGenerado = await connection.QuerySingleAsync<int>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Retorna el ID
                return idGenerado;
            }
        }

        public async Task<bool> CancelarTurnoAsync(int turnoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "sp_EliminarTurno";

                var result = await connection.ExecuteAsync(
                    storedProcedure,
                    new { id_turno = turnoId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0;
            }
        }


    }
}
