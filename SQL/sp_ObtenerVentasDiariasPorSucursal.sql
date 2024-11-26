CREATE PROCEDURE sp_ObtenerVentasDiariasPorSucursal
	@id_sucursal INT,
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FORMAT(CAST(fecha AS DATETIME) + CAST(hora AS DATETIME), 'HH:mm') AS Hora,
        SUM(precio) AS TotalVentas
    FROM 
        turnos
    WHERE 
        fecha = @Fecha AND ausente = 0 and id_sucursal = @id_sucursal
    GROUP BY 
        CAST(fecha AS DATETIME) + CAST(hora AS DATETIME); -- Agrupar por la fecha y hora combinadas
END;
