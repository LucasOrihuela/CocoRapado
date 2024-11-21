CREATE PROCEDURE sp_ObtenerServiciosPorTurnos
    @IdTurno INT
AS
BEGIN
    SELECT
	id as Id,
	servicio as ServicioNombre,
	descripcion as ServicioDescripcion,
	duracion_min as DuracionMinutos,
	precio_min as PrecioMin,
	precio_max as PrecioMax,
	imagen as Imagen,
	sxs.id_sucursal as IdSucursal
	FROM servicios s
	INNER JOIN servicios_x_turno st on st.id_servicio = s.id
	LEFT JOIN servicio_x_sucursal sxs on sxs.id_servicio = s.id
	WHERE st.id_turno = @IdTurno
END