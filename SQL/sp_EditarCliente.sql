CREATE PROCEDURE sp_EditarCliente
	@id_cliente INT,
    @Correo VARCHAR(255),
    @Imagen VARCHAR(500),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @Telefono VARCHAR(30),
	@FechaNacimiento DATE
AS
BEGIN
	DECLARE @id_perfil INT

	SELECT @id_perfil=id FROM perfiles WHERE perfiles.rol = 'Cliente'

	UPDATE usuarios
	SET correo = @Correo,
		imagen = @Imagen,
		nombre = @Nombre,
		apellido = @Apellido,
		telefono = @Telefono,
		fecha_nacimiento = @FechaNacimiento
		WHERE usuarios.id = @id_cliente and usuarios.id_perfil = @id_perfil

	SELECT @@ROWCOUNT;
END