CREATE PROCEDURE sp_CrearSucursal
    @Nombre VARCHAR(255),
    @Direccion VARCHAR(255),
    @Localidad VARCHAR(255),
    @Imagen VARCHAR(255),
	@Telefono VARCHAR(255),
	@PrecioAbono INT
AS
BEGIN
    INSERT INTO sucursales (nombre, direccion, localidad, imagen, telefono, precioAbono)
    VALUES (@Nombre, @Direccion, @Localidad, @Imagen, @Telefono, @PrecioAbono)
	SELECT SCOPE_IDENTITY();
END 