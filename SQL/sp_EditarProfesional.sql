CREATE PROCEDURE sp_EditarProfesional
	@id_profesional INT,
    @Correo VARCHAR(255),
    @PasswordHash VARCHAR(255),
    @Imagen VARCHAR(500),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @Telefono VARCHAR(30),
	@FechaNacimiento DATE,
    @SecurityStamp VARCHAR(255)
AS
BEGIN
	DECLARE @id_perfil INT

	SELECT @id_perfil=id FROM perfiles WHERE perfiles.rol = 'Profesional'

	UPDATE usuarios
	SET correo = @Correo,
		password_hash = @PasswordHash,
		imagen = @Imagen,
		nombre = @Nombre,
		apellido = @Apellido,
		telefono = @Telefono,
		fecha_nacimiento = @FechaNacimiento,
		security_stamp = @SecurityStamp
		WHERE usuarios.id = @id_profesional and usuarios.id_perfil = @id_perfil

	SELECT @@ROWCOUNT;
END