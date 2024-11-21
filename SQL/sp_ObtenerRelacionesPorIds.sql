CREATE PROCEDURE sp_ObtenerRelacionesPorIds
	@id_profesional INT,
	@id_servicio INT
AS
BEGIN
    SELECT
	u.id as IdProfesional,
	s.id as IdServicio,
	u.nombre as Nombre,
	u.apellido as Apellido,
	s.servicio as ServicioNombre
	FROM servicio_x_profesional sxp
	INNER JOIN usuarios u ON u.id = sxp.id_profesional
	INNER JOIN servicios s ON s.id = sxp.id_servicio
	WHERE sxp.id_profesional = @id_profesional and sxp.id_servicio = @id_servicio
END
