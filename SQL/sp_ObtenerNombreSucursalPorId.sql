CREATE PROCEDURE sp_ObtenerNombreSucursalPorId
    @SucursalId INT
AS
BEGIN
    SELECT 
        Nombre
    FROM 
        Sucursales
    WHERE 
        Id = @SucursalId
END