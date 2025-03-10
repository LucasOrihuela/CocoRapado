CREATE PROCEDURE sp_ObtenerAdministradores
AS
BEGIN
    SELECT
		u.id as id,
		id_perfil as IdPerfil,
		correo as Correo,
		password_hash as PasswordHash,
		imagen as Imagen,
		nombre as Nombre,
		apellido as Apellido,
		telefono as Telefono,
		fecha_nacimiento as FechaNacimiento,
		id_sucursal as IdSucursal
		FROM usuarios u
		INNER JOIN perfiles p ON u.id_perfil = p.id
		INNER JOIN administrador_x_sucursal axs ON axs.id_admin = u.id
		WHERE p.rol = 'Administrador'
END