CREATE PROCEDURE sp_SuscribirClienteAbono
	@id_cliente INT,
	@id_sucursal INT
AS
BEGIN
	
	INSERT INTO abono_cliente_x_sucursal VALUES (@id_cliente, @id_sucursal)

	SELECT SCOPE_IDENTITY();
END