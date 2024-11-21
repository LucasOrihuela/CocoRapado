CREATE PROCEDURE sp_ObtenerTelefonoSucursalPorId
    @SucursalId INT
AS
BEGIN
    SELECT 
        telefono as Telefono
    FROM 
        Sucursales
    WHERE 
        Id = @SucursalId
END 