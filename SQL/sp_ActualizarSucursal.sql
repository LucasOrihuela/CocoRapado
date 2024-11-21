CREATE PROCEDURE sp_ActualizarSucursal
    @Id INT,
    @Nombre VARCHAR(255),
    @Direccion VARCHAR(255),
    @Localidad VARCHAR(255),
    @Imagen VARCHAR(255) = NULL, -- Permitir que el parámetro sea NULL
    @Telefono VARCHAR(50),
	@PrecioAbono INT
AS
BEGIN
    UPDATE sucursales
    SET 
        nombre = @Nombre,
        direccion = @Direccion,
        localidad = @Localidad,
        imagen = CASE 
                    WHEN @Imagen IS NOT NULL THEN @Imagen 
                    ELSE imagen  -- Mantener el valor actual si @Imagen es NULL
                 END,
        telefono = @Telefono,
		precioAbono = @PrecioAbono
    WHERE id = @Id
END