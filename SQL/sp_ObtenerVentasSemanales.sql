CREATE PROCEDURE sp_ObtenerVentasSemanales
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SELECT 
        CAST(fecha AS DATE) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE fecha >= @FechaInicio AND fecha <= @FechaFin AND ausente = 0
    GROUP BY CAST(fecha AS DATE);
END;
