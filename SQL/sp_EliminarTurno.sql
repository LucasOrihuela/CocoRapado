CREATE PROCEDURE sp_EliminarTurno
    @id_turno INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

			-- Elimina la relaci�n entre servicios y turno
			DELETE FROM servicios_x_turno WHERE id_turno = @id_turno;

			-- Elimina el turno
			DELETE FROM turnos WHERE id = @id_turno;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Si ocurre un error, deshace la transacci�n
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
