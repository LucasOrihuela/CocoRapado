CREATE PROCEDURE sp_ObtenerVentasMensuales
    @Mes INT,
    @Anio INT
AS
BEGIN
    SELECT 
        CONCAT(DAY(fecha), '/', MONTH(fecha)) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE MONTH(fecha) = @Mes 
      AND YEAR(fecha) = @Anio 
      AND ausente = 0
    GROUP BY DAY(fecha), MONTH(fecha)
END;