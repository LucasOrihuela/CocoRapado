CREATE PROCEDURE sp_CrearProfesionalFavorito
	@id_usuario INT,
	@id_profesional INT
AS
BEGIN
	INSERT INTO profesionales_favoritos (id_usuario, id_profesional) VALUES(
        @id_usuario,
		@id_profesional
		)
	SELECT SCOPE_IDENTITY();
END