CREATE PROCEDURE sp_CrearHorario
    @IdSucursal INT,
    @Dia VARCHAR(100),
    @HorarioApertura VARCHAR(100),
    @HorarioCierreMediodia VARCHAR(100),
    @HorarioAperturaMediodia VARCHAR(100),
    @HorarioCierre VARCHAR(100)
AS
BEGIN
    INSERT INTO horarios (id_sucursal, dia, horario_apertura, horario_cierre_mediodia, horario_apertura_mediodia, horario_cierre)
    VALUES (@IdSucursal, @Dia, @HorarioApertura, @HorarioCierreMediodia, @HorarioAperturaMediodia, @HorarioCierre);
END
