CREATE PROCEDURE sp_ObtenerSucursalesPorProfesionalSinAsignar
    @IdProfesional INT
AS
BEGIN
    SELECT DISTINCT
	s.id as Id,
	s.nombre as Nombre
	FROM sucursales s
	WHERE s.id not in(SELECT id_sucursal FROM profesional_x_sucursal WHERE id_profesional = @IdProfesional) 
END