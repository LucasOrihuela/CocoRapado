using Dapper;
using Cocorapado.Models;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProfesionalesPorSucursalService
{
    private readonly IDbConnection _connection;

    // Constructor que recibe IDbConnection
    public ProfesionalesPorSucursalService(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<ProfesionalesPorSucursales>> ObtenerRelacionesAsync()
    {
        var relaciones = new List<ProfesionalesPorSucursales>();

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
                command.CommandText = "sp_ObtenerProfesionalesPorSucursales";
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var relacion = new ProfesionalesPorSucursales
                        {
                            IdProfesional = reader.IsDBNull(reader.GetOrdinal("IdProfesional")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                            IdSucursal = reader.IsDBNull(reader.GetOrdinal("IdSucursal")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdSucursal")),
                            NombreProfesional = reader.IsDBNull(reader.GetOrdinal("NombreProfesional")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreProfesional")),
                            ApellidoProfesional = reader.IsDBNull(reader.GetOrdinal("ApellidoProfesional")) ? string.Empty : reader.GetString(reader.GetOrdinal("ApellidoProfesional")),
                            NombreSucursal = reader.IsDBNull(reader.GetOrdinal("NombreSucursal")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreSucursal"))
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

    public async Task<List<UsuarioDTO>> ObtenerProfesionalesPorSucursalYServicioAsync(int idSucursal, List<int> idsServicios)
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

        // Si no hay servicios seleccionados, llamamos al SP sin la lista de servicios
        if (idsServicios == null || !idsServicios.Any())
        {
            var resultados = (await sqlConnection.QueryAsync<UsuarioDTO>(
                "sp_ObtenerProfesionalesPorSucursalYServicio",
                new { id_sucursal = idSucursal, ids_servicios = (string)null }, // Se pasa null para los servicios
                commandType: CommandType.StoredProcedure
            )).ToList();

            return resultados;
        }

        // Convertir la lista de servicios a un string para usar en la consulta
        var idsServiciosString = string.Join(",", idsServicios);

        // Ejecutar la consulta
        var resultadosConServicios = (await sqlConnection.QueryAsync<UsuarioDTO>(
            "sp_ObtenerProfesionalesPorSucursalYServicio",
            new { id_sucursal = idSucursal, ids_servicios = idsServiciosString },
            commandType: CommandType.StoredProcedure
        )).ToList();

        return resultadosConServicios;
    }



    // Método para obtener el nombre del sucursal por ID
    public async Task<string?> ObtenerSucursalPorIdAsync(int idSucursal)
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
                "sp_ObtenerSucursalPorId",
                new { IdSucursal = idSucursal },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al obtener sucursal por ID", ex);
        }
    }

    public async Task<ProfesionalesPorSucursales> ObtenerRelacionPorIdsAsync(int idProfesional, int idSucursal)
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

            var result = await sqlConnection.QuerySingleOrDefaultAsync<ProfesionalesPorSucursales>(
                "sp_ObtenerProfesionalesPorSucursalesPorIds",
                new { id_Profesional = idProfesional, id_sucursal = idSucursal },
                commandType: CommandType.StoredProcedure
            );

            return result ?? new ProfesionalesPorSucursales(); // Retorna una instancia vacía si result es null
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al obtener la relación por IDs", ex);
        }
    }

    public async Task<bool> EliminarRelacionAsync(int id_profesional, int id_sucursal)
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
                "sp_EliminarProfesionalPorSucursal",
                new { id_profesional, id_sucursal },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0; // Devuelve verdadero si se eliminó al menos una fila
        }
        catch (SqlException ex)
        {
            throw new Exception("Error al eliminar la relación", ex);
        }
    }

    public async Task<int> CrearRelacionAsync(int id_profesional, int id_sucursal)
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
                "sp_CrearProfesionalPorSucursal",
                new { id_profesional, id_sucursal },
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
