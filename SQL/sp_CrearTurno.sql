CREATE PROCEDURE sp_CrearTurno
    @fecha DATE,
	@hora TIME,
	@idSucursal INT,
	@idProfesional INT,
	@idCliente INT,
	@precio INT,
	@duracion_min INT
AS
BEGIN
    INSERT INTO turnos (fecha,hora,id_sucursal,id_profesional,id_cliente,precio,duracion_min,ausente) VALUES(
        @fecha,
		@hora,
		@idSucursal,
		@idProfesional,
		@idCliente,
		@precio,
		@duracion_min,
		0
		)
	SELECT SCOPE_IDENTITY();
END