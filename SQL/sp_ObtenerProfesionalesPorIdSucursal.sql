CREATE PROCEDURE sp_ObtenerProfesionalesPorIdSucursal
	@id_sucursal INT
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
		INNER JOIN profesional_x_sucursal pxs ON pxs.id_profesional = u.id
		WHERE p.rol like '%Profesional%' AND pxs.id_sucursal = @id_sucursal
END
