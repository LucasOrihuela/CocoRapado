CREATE PROCEDURE sp_ObtenerTurnosPorProfesional
    @IdProfesional INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.id AS Id,
        t.fecha AS Fecha,
        t.hora AS Hora,
        t.id_sucursal AS IdSucursal,
        t.id_profesional AS IdProfesional,
        t.id_cliente AS IdCliente,
        t.precio AS Precio,
        t.duracion_min AS DuracionMin,
        s.nombre AS SucursalNombre,
        t.ausente AS Ausente,
        CONCAT(u.nombre, ' ', u.apellido) AS ClienteNombre,
        -- Subconsulta para verificar si el cliente estuvo ausente en su turno anterior
        ISNULL(
            (SELECT TOP 1 
                 ausente
             FROM 
                 turnos tPrev
             WHERE 
                 tPrev.id_cliente = t.id_cliente 
                 AND tPrev.fecha < t.fecha -- Turno anterior
             ORDER BY 
                 tPrev.fecha DESC, tPrev.hora DESC), 0) AS AusenteTurnoAnterior
    FROM 
        turnos t
    INNER JOIN 
        sucursales s ON s.id = t.id_sucursal
    INNER JOIN 
        usuarios u ON u.id = t.id_cliente
    WHERE 
        t.id_profesional = @IdProfesional
    ORDER BY 
        t.fecha DESC, t.hora DESC; -- Ordenar por fecha y hora
END;
