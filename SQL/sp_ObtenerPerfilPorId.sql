CREATE PROCEDURE sp_ObtenerRolPorId
	@id INT
AS
BEGIN
	SELECT
		rol as Rol
	FROM perfiles
	WHERE id = @id
END