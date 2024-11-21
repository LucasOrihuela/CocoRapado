CREATE PROCEDURE sp_EditarServicio
	@IdServicio INT,
    @ServicioNombre VARCHAR(200),
	@ServicioDescripcion VARCHAR(500),
	@DuracionMinutos INT,
    @PrecioMin INT,
    @PrecioMax INT,
    @Imagen VARCHAR(500)
AS
BEGIN

	UPDATE servicios
	SET servicio = @ServicioNombre,
		descripcion = @ServicioDescripcion,
		duracion_min = @DuracionMinutos,
		precio_min = @PrecioMin,
		precio_max = @PrecioMax,
		imagen = @Imagen
		WHERE servicios.id = @IdServicio

	SELECT @@ROWCOUNT;
END