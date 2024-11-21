CREATE PROCEDURE sp_ObtenerNombreServicioPorId
    @IdServicio INT
AS
BEGIN
    SELECT
        servicio as Nombre
    FROM 
        servicios
    WHERE 
        id = @IdServicio
END