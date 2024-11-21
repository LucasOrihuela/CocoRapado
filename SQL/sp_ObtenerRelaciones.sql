CREATE PROCEDURE sp_ObtenerRelaciones
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
END
