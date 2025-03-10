using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cocorapado.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Cocorapado.Service
{
    public class UsuarioService
    {
        private readonly IDbConnection _connection;
        private readonly UserManager<Usuario> _userManager;

        // Constructor que recibe UserManager y IDbConnection
        public UsuarioService(UserManager<Usuario> userManager, IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IEnumerable<Usuario>> GetProfesionalesByServicioIdAsync(int servicioId, int sucursalId)
        {
            var query = "EXEC sp_ObtenerProfesionalesPorServicio @ServicioId, @SucursalId";
            var parameters = new { ServicioId = servicioId, SucursalId = sucursalId };
            var result = await _connection.QueryAsync<Usuario>(query, parameters);
            return result;
        }

        public async Task<IEnumerable<Usuario>> GetProfesionalesBySucursalIdAsync(int sucursalId)
        {
            var query = "sp_ObtenerProfesionalesPorIdSucursal";
            var parameters = new { id_sucursal = sucursalId };
            var result = await _connection.QueryAsync<Usuario>(query, parameters);
            return result;
        }

        public async Task<String?> GetNombreProfesionalesByIdAsync(int idProfesional)
        {
            var nombre = await _connection.QueryFirstOrDefaultAsync<String>(
            "sp_ObtenerNombreProfesionalPorId",
                 new { IdProfesional = idProfesional },
                 commandType: CommandType.StoredProcedure
             );

            return nombre;
        }

        public async Task<int> GetTelefonoClienteByIdAsync(string idClienteString)
        {
            var idCliente = 0;
            var resultado = 0;

            int.TryParse(idClienteString, out idCliente);
            var telefono = await _connection.QueryFirstOrDefaultAsync<int>(
            "sp_ObtenerTelefonoClientePorId",
                 new { IdCliente = idCliente },
                 commandType: CommandType.StoredProcedure
             );

            

            return resultado;
        }

        public async Task<Usuario?> Authenticate(string email, string clave)
        {
            // Llamada al stored procedure sp_ObtenerUsuarioPorCorreo
            var usuario = await _connection.QueryFirstOrDefaultAsync<Usuario>(
                "sp_ObtenerUsuarioPorCorreo", // Nombre del stored procedure
                new { Correo = email }, // Parámetros para el stored procedure
                commandType: CommandType.StoredProcedure // Indicamos que es un stored procedure
            );

            // Si el usuario no existe, retorna null
            if (usuario == null)
            {
                return null;
            }

            // Verifica si el PasswordHash no es nulo
            if (string.IsNullOrEmpty(usuario.PasswordHash))
            {
                return null; // Retorna null si no hay hash de contraseña
            }

            // Verifica la contraseña
            var passwordHasher = new PasswordHasher<Usuario>();
            var result = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, clave);

            if (result == PasswordVerificationResult.Success)
            {
                return usuario; // Retorna el usuario si la autenticación fue exitosa
            }

            return null; // Retorna null si la contraseña no es correcta
        }

        public async Task<int> CrearUsuarioAsync(Usuario usuario)
        {
            // Genera el hash de la contraseña usando la propiedad Clave
            var passwordHasher = new PasswordHasher<Usuario>();
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, usuario.Clave);

            if (usuario.IdPerfil == 0)
            {
                usuario.IdPerfil = 1;
            }


            var query = "sp_CrearUsuario";

            var parameters = new DynamicParameters();
            parameters.Add("@IdPerfil", usuario.IdPerfil);
            parameters.Add("@Correo", usuario.Correo);
            parameters.Add("@PasswordHash", usuario.PasswordHash);
            parameters.Add("@Imagen", usuario.Imagen);
            parameters.Add("@Nombre", usuario.Nombre);
            parameters.Add("@Apellido", usuario.Apellido);
            parameters.Add("@Telefono", usuario.Telefono);
            parameters.Add("@Fecha_nacimiento", usuario.FechaNacimiento);
            parameters.Add("@SecurityStamp", usuario.SecurityStamp);
            parameters.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@NuevoUsuarioId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            try
            {
                // Ejecutar el procedimiento almacenado
                await _connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

                // Obtener los resultados de los parámetros de salida
                var resultado = parameters.Get<int>("@Resultado");
                var nuevoUsuarioId = parameters.Get<int>("@NuevoUsuarioId");

                // Manejar el resultado
                if (resultado != 0) // Verifica si hay un error
                {
                    // Lanza excepciones dependiendo del error
                    if (resultado == 1)
                    {
                        throw new Exception("El correo ya existe en la misma sucursal.");
                    }
                    else if (resultado == 2)
                    {
                        throw new Exception("El correo ya existe.");
                    }
                    else if (resultado == -1)
                    {
                        throw new Exception("Error interno al crear el usuario.");
                    }
                }

                return nuevoUsuarioId; // Retorna el ID del nuevo usuario creado
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Perfil?> ObtenerPerfilPorUsuarioAsync(int idUsuario)
        {
            var query = "sp_ObtenerPerfilPorUsuario";
            var parameters = new { id_usuario = idUsuario };

            var perfil = await _connection.QueryFirstOrDefaultAsync<Perfil>(
                query,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return perfil;
        }

        public async Task<IEnumerable<Usuario>> GetTodosLosProfesionalesAsync()
        {
            var query = "sp_ObtenerProfesionales";
            var result = await _connection.QueryAsync<Usuario>(query);
            return result;
        }

        public async Task<bool> EliminarProfesionalAsync(int id)
        {
            // Usar la conexión directamente en lugar de crear una nueva
            var result = await _connection.ExecuteAsync("sp_EliminarProfesional", new { id = id }, commandType: CommandType.StoredProcedure);
            return result > 0; // Devuelve verdadero si se eliminó al menos una fila
        }

        public async Task<bool> EditarProfesionalAsync(Usuario profesional) // Cambia a Task<bool>
        {
            // Genera el hash de la contraseña usando la propiedad Clave
            var passwordHasher = new PasswordHasher<Usuario>();
            profesional.PasswordHash = passwordHasher.HashPassword(profesional, profesional.Clave);

            var result = await _connection.ExecuteAsync("sp_EditarProfesional", new
            {
                id_profesional = profesional.Id,
                Correo = profesional.Correo,
                PasswordHash = profesional.PasswordHash,
                Imagen = profesional.Imagen,
                Nombre = profesional.Nombre,
                Apellido = profesional.Apellido,
                Telefono = profesional.Telefono,
                FechaNacimiento = profesional.FechaNacimiento,
                SecurityStamp = profesional.SecurityStamp
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<bool> SuscribirClienteAbono(int idCliente, int idSucursal) // Cambia a Task<bool>
        {
            var result = await _connection.ExecuteAsync("sp_SuscribirClienteAbono", new
            {
                id_cliente = idCliente,
                id_sucursal = idSucursal
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<bool> VerificarUsuarioBloqueado(int idUsuario)
        {
            var result = await _connection.ExecuteScalarAsync<int>("sp_VerificaUsuarioBloqueado", new
            {
                id_usuario = idUsuario
            }, commandType: CommandType.StoredProcedure);

            return result == 1;
        }

        public async Task<bool> CancelarSuscripcionClienteAbono(int idCliente, int idSucursal) // Cambia a Task<bool>
        {
            var result = await _connection.ExecuteAsync("sp_CancelarSuscripcionAbono", new
            {
                id_cliente = idCliente,
                id_sucursal = idSucursal
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<bool> TieneAbonoAsync(int idCliente, int idSucursal)
        {
            // Usa QuerySingleOrDefaultAsync para obtener un resultado
            var result = await _connection.QuerySingleOrDefaultAsync<int?>(
                "sp_TieneAbono",
                new
                {
                    id_cliente = idCliente,
                    id_sucursal = idSucursal
                },
                commandType: CommandType.StoredProcedure
            );

            // Si el resultado no es nulo, existe un abono
            return result.HasValue;
        }


        public async Task<bool> EditarClienteAsync(Usuario cliente)
        {


            var result = await _connection.ExecuteAsync("sp_EditarCliente", new
            {
                id_cliente = cliente.Id,
                Correo = cliente.Correo,
                Imagen = cliente.Imagen,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Telefono = cliente.Telefono,
                FechaNacimiento = cliente.FechaNacimiento
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<UsuarioDTO> ObtenerProfesionalPorIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id_profesional", id);

            string sql = "sp_ObtenerProfesionalPorId";

            // Ejecutar el stored procedure y devolver el resultado
            var resultado = await _connection.QuerySingleOrDefaultAsync<Usuario>(sql, parameters, commandType: CommandType.StoredProcedure);

            if (resultado == null)
            {
                throw new KeyNotFoundException($"No se encontró un profesional con ID {id}.");
            }

            // Mapear el resultado a un UsuarioDTO antes de devolverlo
            var usuarioDTO = new UsuarioDTO
            {
                Id = id,
                IdPerfil = resultado.IdPerfil,
                Correo = resultado.Correo,
                Imagen = resultado.Imagen,
                Nombre = resultado.Nombre,
                Apellido = resultado.Apellido,
                Telefono = resultado.Telefono,
                FechaNacimiento = resultado.FechaNacimiento,
                EstaLogueado = resultado.EstaLogueado
            };

            return usuarioDTO;
        }

        public async Task<List<int>> GetProfesionalesFavoritosAsync(int idCliente)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id_usuario", idCliente);

            string sql = "sp_ObtenerProfesionalesFavoritos";

            var resultado = await _connection.QueryAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);

            return resultado.ToList();
        }

        public async Task<bool> CrearProfesionalFavoritoAsync(int idCliente, int idProfesional)
        {
            var query = "sp_CrearProfesionalFavorito";
            bool resultado = false;
            var parameters = new DynamicParameters();
            parameters.Add("@id_usuario", idCliente);
            parameters.Add("@id_profesional", idProfesional);

            try
            {
                var idRegistro = await _connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);


                if (idRegistro != 0)
                {
                    resultado = true;
                }
                else
                {
                    throw new Exception("El correo ya existe en la misma sucursal.");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> EliminarProfesionalFavoritoAsync(int idCliente, int idProfesional)
        {
            // Usar la conexión directamente en lugar de crear una nueva
            var result = await _connection.ExecuteAsync("sp_EliminarProfesionalFavorito", new { id_usuario = idCliente, id_profesional = idProfesional }, commandType: CommandType.StoredProcedure);
            return result > 0; // Devuelve verdadero si se eliminó al menos una fila
        }

        public async Task<UsuarioDTO> ObtenerAdministradorPorIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id_administrador", id);

            string sql = "sp_ObtenerAdministradorPorId";

            // Ejecutar el stored procedure y devolver el resultado
            var resultado = await _connection.QuerySingleOrDefaultAsync<Usuario>(sql, parameters, commandType: CommandType.StoredProcedure);

            if (resultado == null)
            {
                throw new KeyNotFoundException($"No se encontró un profesional con ID {id}.");
            }

            // Mapear el resultado a un UsuarioDTO antes de devolverlo
            var usuarioDTO = new UsuarioDTO
            {
                Id = id,
                IdPerfil = resultado.IdPerfil,
                Correo = resultado.Correo,
                Imagen = resultado.Imagen,
                Nombre = resultado.Nombre,
                Apellido = resultado.Apellido,
                Telefono = resultado.Telefono,
                FechaNacimiento = resultado.FechaNacimiento,
                EstaLogueado = resultado.EstaLogueado
            };

            return usuarioDTO;
        }

        public async Task<bool> EditarAdministradorAsync(Usuario administrador) // Cambia a Task<bool>
        {
            // Genera el hash de la contraseña usando la propiedad Clave
            var passwordHasher = new PasswordHasher<Usuario>();
            administrador.PasswordHash = passwordHasher.HashPassword(administrador, administrador.Clave);

            var result = await _connection.ExecuteAsync("sp_EditarAdministrador", new
            {
                id_profesional = administrador.Id,
                Correo = administrador.Correo,
                PasswordHash = administrador.PasswordHash,
                Imagen = administrador.Imagen,
                Nombre = administrador.Nombre,
                Apellido = administrador.Apellido,
                Telefono = administrador.Telefono,
                FechaNacimiento = administrador.FechaNacimiento,
                SecurityStamp = administrador.SecurityStamp
            }, commandType: CommandType.StoredProcedure);

            return result > 0; // Devuelve verdadero si se actualizó al menos una fila
        }

        public async Task<bool> EliminarAdministradorAsync(int idAdministrador)
        {
            var result = await _connection.ExecuteAsync("sp_EliminarAdministrador", new {id_administrador = idAdministrador }, commandType: CommandType.StoredProcedure);
            return result > 0; // Devuelve verdadero si se eliminó al menos una fila
        }

        public async Task<int> GetSucursalByAdministradorIdAsync(int administradorId)
        {
            var query = "sp_ObtenerSucursalPorAdministrador";
            var parameters = new { id_admin = administradorId };

            var result = await _connection.QueryAsync<int>(query, parameters);

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Usuario>> GetAdministradoresBySucursalIdAsync(int sucursalId)
        {
            var query = "sp_ObtenerAdministradoresPorIdSucursal";
            var parameters = new { id_sucursal = sucursalId };
            var result = await _connection.QueryAsync<Usuario>(query, parameters);
            return result;
        }

        public async Task<IEnumerable<Administrador>> GetAdministradoresAsync()
        {
            var query = "sp_ObtenerAdministradores";
            var result = await _connection.QueryAsync<Administrador>(query);
            return result;
        }


        public async Task<Usuario> ObtenerUsuarioPorCorreoAsync(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@correo", email);

            string sql = "sp_ObtenerUsuarioPorCorreo";

            // Ejecutar el stored procedure y devolver el resultado
            var resultado = await _connection.QuerySingleOrDefaultAsync<Usuario>(sql, parameters, commandType: CommandType.StoredProcedure);

            if (resultado == null)
            {
                throw new KeyNotFoundException($"No se encontró un usuario con correo {email}.");
            }

            // Mapear el resultado a un Usuario antes de devolverlo
            var usuario = new Usuario
            {
                Id = resultado.Id,
                IdPerfil = resultado.IdPerfil,
                Correo = resultado.Correo,
                Imagen = resultado.Imagen,
                Nombre = resultado.Nombre,
                Apellido = resultado.Apellido,
                Telefono = resultado.Telefono,
                FechaNacimiento = resultado.FechaNacimiento,
                EstaLogueado = resultado.EstaLogueado
            };

            return usuario;
        }

    }
    
}
