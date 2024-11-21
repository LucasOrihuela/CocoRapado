CREATE PROCEDURE sp_ObtenerProfesionalesPorSucursalYServicio
    @id_sucursal INT,
    @ids_servicios VARCHAR(MAX) = NULL -- Recibe una lista de servicios como una cadena, por defecto es NULL
AS
BEGIN
    IF @ids_servicios IS NULL OR @ids_servicios = ''
    BEGIN
        -- Si no hay servicios seleccionados, devuelve todos los profesionales de la sucursal
        SELECT 
			pxs.id_profesional AS Id,
            u.nombre AS Nombre,
            u.apellido AS Apellido
        FROM profesional_x_sucursal pxs
        INNER JOIN usuarios u ON u.id = pxs.id_profesional
        WHERE pxs.id_sucursal = @id_sucursal
        GROUP BY pxs.id_profesional, u.nombre, u.apellido;
    END
    ELSE
    BEGIN
        -- Si hay servicios seleccionados, devuelve los profesionales que ofrecen todos ellos
        SELECT 
            pxs.id_profesional AS Id,
            u.nombre AS Nombre,
            u.apellido AS Apellido
        FROM profesional_x_sucursal pxs
        INNER JOIN usuarios u ON u.id = pxs.id_profesional
        INNER JOIN servicio_x_profesional sxp ON sxp.id_profesional = pxs.id_profesional
        WHERE pxs.id_sucursal = @id_sucursal AND sxp.id_servicio IN (SELECT value FROM STRING_SPLIT(@ids_servicios, ','))
        GROUP BY  pxs.id_profesional, u.nombre, u.apellido
        HAVING COUNT(sxp.id_servicio) = (SELECT COUNT(value) FROM STRING_SPLIT(@ids_servicios, ','));
    END
END
