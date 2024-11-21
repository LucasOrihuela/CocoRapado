CREATE PROCEDURE sp_ObtenerServiciosPorSucursal
    @IdSucursal INT
AS
BEGIN
    SELECT 
        s.id as Id,
        s.servicio as ServicioNombre,
        s.descripcion as ServicioDescripcion,
        s.duracion_min as DuracionMinutos,
        s.precio_min as PrecioMin,
        s.precio_max as PrecioMax,
        s.Imagen as Imagen,
        CASE 
            WHEN sxs.id_servicio IS NOT NULL THEN 1
            ELSE 0
        END as Checked
    FROM servicios s
    LEFT JOIN servicio_x_sucursal sxs ON sxs.id_servicio = s.id AND sxs.id_sucursal = @IdSucursal;
END