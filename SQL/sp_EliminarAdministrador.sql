CREATE PROCEDURE sp_EliminarAdministrador
    @id INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        DECLARE @id_perfil INT

        SELECT @id_perfil = id FROM perfiles WHERE rol = 'Administrador';

        IF @id_perfil IS NULL
        BEGIN
            RAISERROR('El perfil con rol "Administrador" no fue encontrado.', 16, 1);
            RETURN;
        END

		-- Elimina la relación entre sucursal y el administrador
		DELETE FROM administrador_x_sucursal WHERE id_admin = @id;

        -- Elimina al profesional
        DELETE FROM usuarios WHERE id_perfil = @id_perfil AND id = @id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Si ocurre un error, deshace la transacción
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Captura el error y lanza un mensaje personalizado
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT @ErrorMessage = ERROR_MESSAGE(), 
               @ErrorSeverity = ERROR_SEVERITY(), 
               @ErrorState = ERROR_STATE();

        -- Lanza el error capturado
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
