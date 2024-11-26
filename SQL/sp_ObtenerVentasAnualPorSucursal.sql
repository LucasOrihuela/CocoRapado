CREATE PROCEDURE sp_ObtenerVentasAnualPorSucursal
	@id_sucursal INT,
    @Anio INT
AS
BEGIN
    SELECT 
        MONTH(fecha) AS Dia,
        SUM(precio) AS TotalVentas
    FROM turnos
    WHERE YEAR(fecha) = @Anio 
      AND ausente = 0
      AND id_sucursal = @id_sucursal 
    GROUP BY MONTH(fecha)
    ORDER BY Dia;
END;
