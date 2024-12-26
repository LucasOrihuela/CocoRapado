CREATE PROCEDURE sp_ObtenerIdPorRol
    @rol VARCHAR(100)
AS
BEGIN
    SELECT
        id AS ID
    FROM perfiles
    WHERE rol = @rol
END
