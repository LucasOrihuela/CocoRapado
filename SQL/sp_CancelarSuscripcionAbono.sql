CREATE PROCEDURE sp_CancelarSuscripcionAbono
	@id_cliente INT,
	@id_sucursal INT
AS
BEGIN	
	BEGIN TRY
        BEGIN TRANSACTION;

        DELETE abono_cliente_x_sucursal WHERE id_cliente = @id_cliente and id_sucursal = @id_sucursal

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Capturar el error y lanzar un mensaje personalizado
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT @ErrorMessage = ERROR_MESSAGE(), 
               @ErrorSeverity = ERROR_SEVERITY(), 
               @ErrorState = ERROR_STATE();

        -- Lanzar el error capturado
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END