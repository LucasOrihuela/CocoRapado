CREATE PROCEDURE sp_ObtenerHorariosPorSucursal
    @IdSucursal INT
AS
BEGIN
    -- Verificar si existen horarios para la sucursal
    IF NOT EXISTS (SELECT 1 FROM horarios WHERE id_sucursal = @IdSucursal)
    BEGIN
        -- Devolver 00:00 para cada día si no hay horarios registrados para la sucursal
        SELECT 
            Dia,
            '00:00' AS HorarioApertura,
            '00:00' AS HorarioCierreMediodia,
            '00:00' AS HorarioAperturaMediodia,
            '00:00' AS HorarioCierre
        FROM 
            (VALUES ('Lunes'), ('Martes'), ('Miercoles'), ('Jueves'), ('Viernes'), ('Sabado'), ('Domingo')) AS Dias(Dia);
    END
    ELSE
    BEGIN
        -- Tabla temporal para almacenar los días de la semana
        DECLARE @DiasDeLaSemana TABLE (Dia VARCHAR(100));

        -- Insertar los 7 días de la semana en la tabla temporal
        INSERT INTO @DiasDeLaSemana (Dia)
        VALUES ('Lunes'), ('Martes'), ('Miercoles'), ('Jueves'), ('Viernes'), ('Sabado'), ('Domingo');

        -- Seleccionar los horarios de la sucursal
        SELECT 
            d.Dia,
            COALESCE(h.horario_apertura, '00:00') AS HorarioApertura,
            COALESCE(h.horario_cierre_mediodia, '00:00') AS HorarioCierreMediodia,
            COALESCE(h.horario_apertura_mediodia, '00:00') AS HorarioAperturaMediodia,
            COALESCE(h.horario_cierre, '00:00') AS HorarioCierre
        FROM 
            @DiasDeLaSemana d
        LEFT JOIN 
            horarios h ON d.Dia = h.dia AND h.id_sucursal = @IdSucursal;
    END
END;
