CREATE PROCEDURE sp_ObtenerDuracionServicioPorId
    @IdServicio INT
AS
BEGIN
    SELECT
        duracion_min as DuracionMin
    FROM 
        servicios
    WHERE 
        id = @IdServicio
END