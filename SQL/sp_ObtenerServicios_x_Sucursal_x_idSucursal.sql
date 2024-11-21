CREATE PROCEDURE sp_ObtenerServicios_x_Sucursal_x_idSucursal
	@IdSucursal INT
AS
BEGIN
    SELECT
	id_servicio as IdServicio
	FROM servicio_x_sucursal
	WHERE id_sucursal = @IdSucursal
END
