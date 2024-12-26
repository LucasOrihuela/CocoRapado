CREATE PROCEDURE sp_ObtenerAdministradorPorId
	@id_administrador INT
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
		fecha_nacimiento as FechaNacimiento
		FROM usuarios u
		INNER JOIN perfiles p ON u.id_perfil = p.id
		WHERE p.rol like '%Administrador%' and u.id = @id_administrador
END
