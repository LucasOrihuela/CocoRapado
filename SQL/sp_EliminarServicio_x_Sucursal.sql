CREATE PROCEDURE sp_EliminarServicio_x_Sucursal
	@id_sucursal INT
AS
BEGIN
	DELETE servicio_x_sucursal WHERE id_sucursal = @id_sucursal
END