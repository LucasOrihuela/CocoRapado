CREATE PROCEDURE sp_SucursalExiste
    @Id INT
AS
BEGIN
    SELECT COUNT(1)
    FROM sucursales
    WHERE id = @Id
END