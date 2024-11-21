CREATE PROCEDURE sp_ObtenerProfesionalesPorSucursales
AS
BEGIN
    SELECT
	pxs.id_profesional as IdProfesional,
	pxs.id_sucursal as IdSucursal,
	u.nombre as NombreProfesional,
	u.apellido as ApellidoProfesional,
	s.nombre as NombreSucursal
	FROM profesional_x_sucursal pxs
	INNER JOIN usuarios u ON u.id = pxs.id_profesional
	INNER JOIN sucursales s ON s.id = pxs.id_sucursal
END
