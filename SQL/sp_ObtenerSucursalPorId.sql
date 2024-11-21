CREATE PROCEDURE sp_ObtenerSucursalPorId
    @SucursalId INT
AS
BEGIN
    SELECT 
        id, 
        Nombre, 
        Direccion, 
        Localidad, 
        Imagen,
		telefono,
		precioAbono
    FROM 
        Sucursales
    WHERE 
        Id = @SucursalId
END