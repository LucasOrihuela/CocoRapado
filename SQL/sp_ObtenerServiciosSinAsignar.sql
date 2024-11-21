CREATE PROCEDURE sp_ObtenerServiciosSinAsignar
    @id_profesional INT
AS
BEGIN
    SELECT DISTINCT
	s.id as Id,
	s.servicio as ServicioNombre
	FROM servicios s
	WHERE s.id not in(SELECT id_servicio FROM servicio_x_profesional WHERE id_profesional = @id_profesional) 
END