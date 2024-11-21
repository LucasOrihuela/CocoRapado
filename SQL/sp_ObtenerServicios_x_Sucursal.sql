CREATE PROCEDURE sp_CrearServicio_x_Sucursal
	@id_servicio INT,
	@id_sucursal INT
AS
BEGIN
    INSERT INTO servicio_x_sucursal (id_servicio, id_sucursal) values (@id_servicio, @id_sucursal)
END
