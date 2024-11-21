CREATE PROCEDURE sp_ObtenerTurnosPorSucursal
    @IdSucursal INT,
	@IdProfesional INT
AS
BEGIN
    SELECT
	t.id as Id,
	t.fecha as Fecha,
	t.hora as Hora,
	t.id_sucursal as IdSucursal,
	t.id_profesional as IdProfesional,
	t.id_cliente as IdCliente,
	t.precio as Precio,
	t.duracion_min as DuracionMin,
	s.nombre as SucursalNombre,
	CONCAT(u.nombre,' ',u.apellido) as ProfesionalNombre
	FROM turnos t
	INNER JOIN sucursales s on s.id = t.id_sucursal
	INNER JOIN usuarios u on u.id = t.id_profesional
	WHERE id_sucursal = @IdSucursal and id_profesional = @IdProfesional;
END
