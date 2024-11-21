CREATE PROCEDURE sp_ActualizarHorario
    @IdSucursal INT,
	@Dia varchar(100),
	@HorarioApertura varchar(100),
	@HorarioCierreMediodia varchar(100),
	@HorarioAperturaMediodia varchar(100),
	@HorarioCierre varchar(100)
AS
BEGIN
    UPDATE horarios
    SET horario_apertura = @HorarioApertura,
        horario_cierre_mediodia = @HorarioCierreMediodia,
        horario_apertura_mediodia = @HorarioAperturaMediodia,
		horario_cierre = @HorarioCierre
    WHERE id_sucursal = @IdSucursal and dia = @Dia
END