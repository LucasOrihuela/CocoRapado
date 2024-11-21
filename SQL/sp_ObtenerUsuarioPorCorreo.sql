CREATE PROCEDURE sp_ObtenerUsuarioPorCorreo
    @Correo VARCHAR(255)
AS
BEGIN
    SELECT 
	id as Id,
	id_perfil as IdPerfil,
	correo as Correo,
	password_hash as PasswordHash,
	imagen as Imagen,
	nombre as Nombre,
	apellido as Apellido,
	telefono as Telefono,
	fecha_nacimiento as FechaNacimiento,
	normalized_email as NormalizedEmail,
	security_stamp as SecurityStamp
	FROM usuarios WHERE correo = @Correo
END
