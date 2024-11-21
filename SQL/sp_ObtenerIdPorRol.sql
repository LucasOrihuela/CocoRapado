CREATE PROCEDURE sp_ObtenerIdPorRol
	@rol VARCHAR(100)
AS
BEGIN
	SELECT
		id as ID
	FROM perfiles
	WHERE rol like '%' + @rol + '%'
END