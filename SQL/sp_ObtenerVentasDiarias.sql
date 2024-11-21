CREATE PROCEDURE sp_ObtenerVentasDiarias
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Consulta ajustada para concatenar correctamente la fecha y la hora
    SELECT 
        FORMAT(CAST(fecha AS DATETIME) + CAST(hora AS DATETIME), 'HH:mm') AS Hora,
        SUM(precio) AS TotalVentas
    FROM 
        turnos
    WHERE 
        fecha = @Fecha AND ausente = 0 -- Filtrar por fecha y ausente=0
    GROUP BY 
        CAST(fecha AS DATETIME) + CAST(hora AS DATETIME); -- Agrupar por la fecha y hora combinadas
END;
