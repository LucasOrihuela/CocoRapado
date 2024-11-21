CREATE PROCEDURE sp_TieneAbono
    @id_cliente INT,
    @id_sucursal INT
AS
BEGIN
    -- Verifica si existe el registro
    IF EXISTS (
        SELECT 1 
        FROM abono_cliente_x_sucursal 
        WHERE id_cliente = @id_cliente AND id_sucursal = @id_sucursal
    )
    BEGIN
        SELECT 1; -- Retorna 1 si existe
    END
    ELSE
    BEGIN
        SELECT NULL; -- Retorna NULL si no existe
    END
END
