CREATE PROCEDURE sp_ObtenerVentasMensuales
    @Mes INT,
    @Anio INT
AS
BEGIN
    SELECT 
        DAY(fecha) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE MONTH(fecha) = @Mes AND YEAR(fecha) = @Anio AND ausente = 0
    GROUP BY DAY(fecha);
END;
