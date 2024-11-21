CREATE PROCEDURE sp_ObtenerPreciosServicioPorId
    @IdServicio INT
AS
BEGIN
    SELECT
        CONCAT(precio_min,',',precio_max) as Precios
    FROM 
        servicios
    WHERE 
        id = @IdServicio
END