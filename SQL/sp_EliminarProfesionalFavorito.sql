CREATE PROCEDURE sp_EliminarProfesionalFavorito
	@id_usuario INT,
    @id_profesional INT
AS
BEGIN
	BEGIN TRY
        BEGIN TRANSACTION;
			DELETE FROM profesionales_favoritos WHERE id_profesional = @id_profesional and id_usuario = @id_usuario
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


