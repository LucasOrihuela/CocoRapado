using Dapper;
using Cocorapado.Models;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ServiciosPorProfesionalService
{
    private readonly IDbConnection _connection;

    // Constructor que recibe IDbConnection
    public ServiciosPorProfesionalService(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<ServiciosPorProfesionalDTO>> ObtenerRelacionesPorSucursalAsync(int? idSucursal)
    {
        var relaciones = new List<ServiciosPorProfesionalDTO>();

        try
        {
            // Casting a SqlConnection para usar OpenAsync
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            // Abrir la conexión de forma asíncrona
            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = idSucursal.HasValue ? "sp_ObtenerRelacionesPorSucursal" : "sp_ObtenerRelaciones";
                command.CommandType = CommandType.StoredProcedure;

                // Se agrega el parámetro solo si idSucursal no es nulo
                if (idSucursal.HasValue)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@IdSucursal";
                    parameter.Value = idSucursal.Value;
                    command.Parameters.Add(parameter);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var relacion = new ServiciosPorProfesionalDTO
                        {
                            IdServicio = reader.IsDBNull(reader.GetOrdinal("IdServicio")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdServicio")),
                            IdProfesional = reader.IsDBNull(reader.GetOrdinal("IdProfesional")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                            Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? string.Empty : reader.GetString(reader.GetOrdinal("Apellido")),
                            ServicioNombre = reader.IsDBNull(reader.GetOrdinal("ServicioNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("ServicioNombre"))
                        };
                        relaciones.Add(relacion);
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            // Manejar la excepción, loguear el error, etc.
            throw new Exception("Error al obtener relaciones por sucursal", ex);
        }

        return relaciones;
    }

    // Método para obtener el nombre y apellido del profesional por ID
    public async Task<(string Nombre, string Apellido)> ObtenerProfesionalPorIdAsync(int idProfesional)
    {
        try
        {
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            var result = await sqlConnection.QuerySingleOrDefaultAsync<(string Nombre, string Apellido)>(
                "sp_ObtenerProfesionalPorId",
                new { id_profesional = idProfesional },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al obtener profesional por ID", ex);
        }
    }

    // Método para obtener el nombre del servicio por ID
    public async Task<string?> ObtenerServicioPorIdAsync(int idServicio)
    {
        try
        {
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            var result = await sqlConnection.QuerySingleOrDefaultAsync<string>(
                "sp_ObtenerServicioPorId",
                new { IdServicio = idServicio },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al obtener servicio por ID", ex);
        }
    }

    public async Task<ServiciosPorProfesionalDTO> ObtenerRelacionPorIdsAsync(int idProfesional, int idServicio)
    {
        try
        {
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            var result = await sqlConnection.QuerySingleOrDefaultAsync<ServiciosPorProfesionalDTO>(
                "sp_ObtenerRelacionesPorIds",
                new { id_Profesional = idProfesional, id_servicio = idServicio },
                commandType: CommandType.StoredProcedure
            );

            return result ?? new ServiciosPorProfesionalDTO(); // Retorna una instancia vacía si result es null
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al obtener la relación por IDs", ex);
        }
    }

    public async Task<bool> EliminarRelacionAsync(int id_profesional, int id_servicio)
    {
        try
        {
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            // Ejecutar el stored procedure para eliminar la relación
            var rowsAffected = await sqlConnection.ExecuteAsync(
                "sp_EliminarRelacion",
                new { id_profesional, id_servicio },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0; // Devuelve verdadero si se eliminó al menos una fila
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al eliminar la relación", ex);
        }
    }

    public async Task<int> CrearRelacionAsync(int id_profesional, int id_servicio)
    {
        try
        {
            var sqlConnection = _connection as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("La conexión no es una instancia de SqlConnection.");
            }

            if (sqlConnection.State == ConnectionState.Closed)
            {
                await sqlConnection.OpenAsync();
            }

            // Ejecutar el stored procedure para crear la relación
            var nuevoId = await sqlConnection.ExecuteScalarAsync<int>(
                "sp_CrearServicioPorProfesional",
                new { id_profesional, id_servicio },
                commandType: CommandType.StoredProcedure
            );

            return nuevoId; // Devuelve el ID generado por la inserción
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al crear la relación", ex);
        }
    }

}
