CREATE PROCEDURE sp_ObtenerVentasSemanalesPorSucursal
	@id_sucursal INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SELECT 
        CAST(fecha AS DATE) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE fecha >= @FechaInicio AND fecha <= @FechaFin AND ausente = 0 AND id_sucursal = @id_sucursal
    GROUP BY CAST(fecha AS DATE);
END;
