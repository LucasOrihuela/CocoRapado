using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cocorapado.Models;

namespace Cocorapado.Service
{
    public class ServicioService
    {
        private readonly IDbConnection _connection;

        // Constructor que recibe IDbConnection
        public ServicioService(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<IEnumerable<Servicio>> GetTodosLosServiciosAsync()
        {
            var query = "sp_ObtenerServicios"; // Asegúrate de que este stored procedure exista
            var result = await _connection.QueryAsync<Servicio>(query);
            return result;
        }

        public async Task<int> CrearServicioAsync(Servicio nuevoServicio)
        {
            var query = "sp_CrearServicio"; // Asegúrate de que este stored procedure exista

            var parameters = new
            {
                ServicioNombre = nuevoServicio.ServicioNombre,       // Coincide con @ServicioNombre
                ServicioDescripcion = nuevoServicio.ServicioDescripcion, // Coincide con @ServicioDescripcion
                DuracionMinutos = nuevoServicio.DuracionMinutos,     // Coincide con @DuracionMinutos
                PrecioMin = nuevoServicio.PrecioMin,                  // Coincide con @PrecioMin
                PrecioMax = nuevoServicio.PrecioMax,                  // Coincide con @PrecioMax
                Imagen = nuevoServicio.Imagen,                         // Coincide con @Imagen
            };

            // Ejecutar el stored procedure y obtener el ID del servicio creado
            var id = await _connection.ExecuteScalarAsync<int>(query, parameters);

            return id; // Retorna el ID del servicio creado
        }

        public async Task<bool> EliminarServicioAsync(int id)
        {
            var result = await _connection.ExecuteAsync("sp_EliminarServicio", new { id = id }, commandType: CommandType.StoredProcedure);
            return result > 0; // Devuelve verdadero si se eliminó al menos una fila
        }

        public async Task<bool> EditarServicioAsync(Servicio servicio)
        {
            var result = await _connection.ExecuteAsync("sp_EditarServicio", new
            {
                IdServicio = servicio.Id,
                ServicioNombre = servicio.ServicioNombre,
                ServicioDescripcion = servicio.ServicioDescripcion,
                DuracionMinutos = servicio.DuracionMinutos,
                PrecioMin = servicio.PrecioMin,
                PrecioMax = servicio.PrecioMax,
                Imagen = servicio.Imagen,
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<Servicio?> ObtenerServicioPorIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IdServicio", id); // Cambiado a IdServicio

            string sql = "sp_ObtenerServicioPorId"; // Asegúrate de que este stored procedure exista

            // Ejecutar el stored procedure y devolver el resultado
            var resultado = await _connection.QuerySingleOrDefaultAsync<Servicio>(sql, parameters, commandType: CommandType.StoredProcedure);

            return resultado; // Puede devolver null si no se encuentra el servicio
        }

        // Nuevo método para obtener servicios no asignados a un profesional
        public async Task<IEnumerable<Servicio>> ObtenerServiciosSinAsignarAsync(int idProfesional)
        {
            var parameters = new { id_profesional = idProfesional }; // Definimos los parámetros que se pasarán al stored procedure
            var query = "sp_ObtenerServiciosSinAsignar"; // Nombre del stored procedure

            // Ejecutar el stored procedure y devolver el resultado
            var serviciosSinAsignar = await _connection.QueryAsync<Servicio>(query, parameters, commandType: CommandType.StoredProcedure);

            return serviciosSinAsignar; // Retorna la lista de servicios
        }

        public async Task<String?> GetNombreServicioByIdAsync(int idServicio)
        {
            var nombre = await _connection.QueryFirstOrDefaultAsync<String>(
            "sp_ObtenerNombreServicioPorId",
                 new { IdServicio = idServicio },
                 commandType: CommandType.StoredProcedure
             );

            return nombre;
        }

        public async Task<String> GetPreciosServicioByIdAsync(int idServicio)
        {
            var precios = "0,0";

            precios = await _connection.QueryFirstOrDefaultAsync<String>(
            "sp_ObtenerPreciosServicioPorId",
            new { IdServicio = idServicio },
            commandType: CommandType.StoredProcedure
            );

            return precios;
        }
    }
}
