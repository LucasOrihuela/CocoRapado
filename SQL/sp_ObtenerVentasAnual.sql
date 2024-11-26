CREATE PROCEDURE sp_ObtenerVentasAnual
    @Anio INT
AS
BEGIN
    SELECT 
        MONTH(fecha) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE YEAR(fecha) = @Anio 
      AND ausente = 0
    GROUP BY MONTH(fecha)
    ORDER BY Dia;
END;
