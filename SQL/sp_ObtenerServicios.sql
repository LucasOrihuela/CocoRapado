CREATE PROCEDURE sp_ObtenerServicios
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
	FROM servicios
END
