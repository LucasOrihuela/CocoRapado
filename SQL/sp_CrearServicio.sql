CREATE PROCEDURE sp_CrearServicio
    @ServicioNombre VARCHAR(200),
    @ServicioDescripcion VARCHAR(500),
    @DuracionMinutos INT,
    @PrecioMin INT,
    @PrecioMax INT,
    @Imagen VARCHAR(500)
AS
BEGIN
    INSERT INTO servicios (servicio, descripcion, duracion_min, precio_min, precio_max, imagen)
    VALUES (@ServicioNombre, @ServicioDescripcion, @DuracionMinutos, @PrecioMin, @PrecioMax, @Imagen);
    
    -- Retorna el ID del nuevo servicio
    SELECT SCOPE_IDENTITY();
END
