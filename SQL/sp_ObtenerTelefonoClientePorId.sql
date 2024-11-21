CREATE PROCEDURE sp_ObtenerTelefonoClientePorId
    @IdCliente INT
AS
BEGIN
    SELECT 
        telefono as Telefono
    FROM 
        usuarios
    WHERE 
        Id = @IdCliente
END 