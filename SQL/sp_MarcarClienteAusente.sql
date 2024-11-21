CREATE PROCEDURE sp_MarcarClienteAusente
    @id_turno INT
AS
BEGIN
    BEGIN TRY
        DECLARE @cancelaciones INT;
        DECLARE @idUsuario INT;

        SELECT 
            @cancelaciones = u.turnos_cancelados, 
            @idUsuario = u.id 
        FROM usuarios u
        INNER JOIN turnos t ON t.id_cliente = u.id
        WHERE t.id = @id_turno;

        -- Validar si se encontró un usuario relacionado al turno
        IF @idUsuario IS NULL
        BEGIN
            THROW 50001, 'No se encontró un cliente relacionado con el turno.', 1;
        END

		-- Suma 1 en sus turnos cancelados, si tiene 3 se bloquea al usuario
		IF @cancelaciones < 3
		BEGIN
			UPDATE usuarios
			SET turnos_cancelados = turnos_cancelados + 1
			WHERE id = @idUsuario;

			IF (@cancelaciones + 1) = 3
			BEGIN
				UPDATE usuarios
				SET bloqueado = 1
				WHERE id = @idUsuario;
			END
		END
		ELSE
		BEGIN
			UPDATE usuarios
			SET bloqueado = 1
			WHERE id = @idUsuario;
		END     

		-- Marco el turno como ausente
		UPDATE turnos
		SET ausente = 1
		WHERE id = @id_turno

        -- Devolver el número de filas afectadas
        SELECT @@ROWCOUNT AS FilasAfectadas;
    END TRY
    BEGIN CATCH
        -- Manejo de errores
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
