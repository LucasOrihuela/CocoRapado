CREATE PROCEDURE sp_ObtenerProfesionalesFavoritos
	@id_usuario INT
AS
BEGIN
	SELECT
		id_profesional
	FROM profesionales_favoritos
	WHERE
		id_usuario = @id_usuario
END