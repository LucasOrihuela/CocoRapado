CREATE PROCEDURE sp_ObtenerTodasLasSucursales
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
END