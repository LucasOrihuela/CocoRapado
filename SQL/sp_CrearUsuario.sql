CREATE PROCEDURE sp_CrearUsuario
    @IdPerfil INT,
    @Correo VARCHAR(255),
    @PasswordHash VARCHAR(255),
    @Imagen VARCHAR(500),
    @Nombre VARCHAR(50),
    @Apellido VARCHAR(50),
    @Telefono VARCHAR(30),
	@Fecha_nacimiento DATE,
    @SecurityStamp VARCHAR(255),
    @Resultado INT OUTPUT,
    @NuevoUsuarioId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Inicializar variables de resultado
    SET @Resultado = 0;
    SET @NuevoUsuarioId = -1;

    -- Verificar si el correo ya existe
    IF EXISTS (SELECT 1 FROM usuarios WHERE correo = @Correo)
    BEGIN
        SET @Resultado = 1;  -- Correo duplicado
        RETURN;
    END

    BEGIN TRY
        -- Inserción del nuevo usuario
        INSERT INTO usuarios (id_perfil, correo, password_hash, imagen, nombre, apellido, telefono, fecha_nacimiento, turnos_cancelados, security_stamp, abono,bloqueado)
        VALUES (@IdPerfil, @Correo, @PasswordHash, @Imagen, @Nombre, @Apellido, @Telefono, @Fecha_nacimiento, 0, @SecurityStamp, 0, 0);

        SET @NuevoUsuarioId = SCOPE_IDENTITY();  -- Obtiene el ID del nuevo usuario creado
        RETURN;  -- Salir del procedimiento
    END TRY
    BEGIN CATCH
        -- Manejo de errores
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Se puede logear el error si se desea
        -- INSERT INTO ErrorLog (Message, Severity, State) VALUES (@ErrorMessage, @ErrorSeverity, @ErrorState);

        -- Retorna un código de error genérico
        SET @Resultado = -1;  -- Retorna -1 en caso de error genérico
        RETURN;  -- Salir del procedimiento
    END CATCH
END
