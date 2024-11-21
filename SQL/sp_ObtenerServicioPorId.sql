CREATE PROCEDURE sp_ObtenerServicioPorId
	@IdServicio INT
AS
BEGIN
    SELECT
	id as Id,
	servicio as ServicioNombre,
	descripcion as ServicioDescripcion,
	duracion_min as DuracionMinutos,
	precio_min as PrecioMin,
	precio_max as PrecioMax,
	imagen as Imagen
	FROM servicios WHERE servicios.id = @IdServicio
END
