CREATE PROCEDURE sp_EliminarSucursal
    @Id INT
AS
BEGIN
	BEGIN TRY
        BEGIN TRANSACTION;
			DELETE FROM profesional_x_sucursal WHERE id_sucursal = @Id
			DELETE FROM sucursales WHERE id = @Id
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


