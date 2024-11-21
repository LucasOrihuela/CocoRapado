CREATE PROCEDURE sp_ObtenerPerfiles
AS
BEGIN
	SELECT
		id as ID,
		rol as Rol
	FROM perfiles
END