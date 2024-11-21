CREATE PROCEDURE sp_ObtenerProfesionalPorId
	@id_profesional INT
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
		WHERE p.rol like '%Profesional%' and u.id = @id_profesional
END
