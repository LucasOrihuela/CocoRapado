CREATE PROCEDURE sp_ObtenerRelacionesPorSucursal
	@IdSucursal INT
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
	INNER JOIN profesional_x_sucursal pxs ON pxs.id_profesional = u.id
	WHERE pxs.id_sucursal = @IdSucursal
END
