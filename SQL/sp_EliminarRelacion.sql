CREATE PROCEDURE sp_EliminarRelacion
	@id_profesional INT,
	@id_servicio INT
AS
BEGIN
	DELETE servicio_x_profesional WHERE id_profesional = @id_profesional and id_servicio = @id_servicio
END