using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cocorapado.Models;

namespace Cocorapado.Service
{
    public class PerfilService
    {
        private readonly IDbConnection _connection;

        // Constructor que recibe IDbConnection
        public PerfilService(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        // Método para obtener todos los perfiles
        public async Task<IEnumerable<Perfil>> GetTodosLosPerfilesAsync()
        {
            var query = "sp_ObtenerPerfiles"; // Asegúrate de que este stored procedure existe
            var result = await _connection.QueryAsync<Perfil>(query);
            return result;
        }

        // Método para obtener id de perfil por rol
        public async Task<int?> ObtenerIdPorRolAsync(string rol)
        {
            var query = "sp_ObtenerIdPorRol";
            var parameters = new DynamicParameters();
            parameters.Add("@rol", rol);

            var idPerfil = await _connection.QuerySingleOrDefaultAsync<int?>(query, parameters, commandType: CommandType.StoredProcedure);
            return idPerfil;
        }

        // Método para crear un nuevo perfil
        //public async Task<int> CrearPerfilAsync(Perfil nuevoPerfil)
        //{
        //    var query = "sp_CrearPerfil";
        //    var parameters = new
        //    {
        //        nuevoPerfil.Id,
        //        nuevoPerfil.Rol
        //    };

        //    var newProfileId = await _connection.ExecuteScalarAsync<int>(query, parameters);
        //    return newProfileId;
        //}

        //// Método para actualizar un perfil existente
        //public async Task<bool> ActualizarPerfilAsync(Perfil perfil)
        //{
        //    var query = "sp_ActualizarPerfil";
        //    var parameters = new
        //    {
        //        perfil.Id,
        //        perfil.Rol
        //    };

        //    var result = await _connection.ExecuteAsync(query, parameters);
        //    return result > 0;
        //}

        //// Método para eliminar un perfil
        //public async Task<bool> EliminarPerfilAsync(int idPerfil)
        //{
        //    var query = "sp_EliminarPerfil";
        //    var parameters = new { IdPerfil = idPerfil };
        //    var result = await _connection.ExecuteAsync(query, parameters);
        //    return result > 0; // Retorna true si se eliminó con éxito
        //}
    }
}
