CREATE PROCEDURE sp_EliminarHorariosPorSucursal
	@id_sucursal INT
AS
BEGIN
	DELETE horarios WHERE id_sucursal = @id_sucursal
END