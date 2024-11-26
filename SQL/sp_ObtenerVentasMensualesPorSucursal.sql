CREATE PROCEDURE sp_ObtenerVentasMensualesPorSucursal
	@id_sucursal INT,
    @Mes INT,
    @Anio INT
AS
BEGIN
    SELECT 
        DAY(fecha) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE MONTH(fecha) = @Mes AND YEAR(fecha) = @Anio AND ausente = 0 AND id_sucursal = @id_sucursal
    GROUP BY DAY(fecha);
END;
