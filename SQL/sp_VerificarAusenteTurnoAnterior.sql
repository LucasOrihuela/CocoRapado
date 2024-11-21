CREATE PROCEDURE sp_VerificarAusenteTurnoAnterior
    @id_cliente INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Ausente INT;

    SELECT TOP 1 
        @Ausente = ausente
    FROM 
        turnos
    WHERE 
        id_cliente = @id_cliente
    ORDER BY 
        fecha DESC

	SELECT @Ausente

END