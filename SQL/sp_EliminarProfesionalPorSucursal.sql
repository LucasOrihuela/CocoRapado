CREATE PROCEDURE sp_EliminarProfesionalPorSucursal
	@id_profesional INT,
	@id_sucursal INT
AS
BEGIN
	DELETE profesional_x_sucursal WHERE id_profesional = @id_profesional and id_sucursal = @id_sucursal
END