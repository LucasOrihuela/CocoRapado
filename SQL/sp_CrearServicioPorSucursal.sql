CREATE PROCEDURE sp_CrearServicioPorSucursal
    @id_servicio INT,
    @id_sucursal INT
AS
BEGIN
    BEGIN TRY
        -- Realiza el insert
        INSERT INTO servicio_x_sucursal(id_servicio, id_sucursal)
        VALUES (@id_servicio, @id_sucursal);

        -- Si la inserción fue exitosa, devuelve 1
        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        -- En caso de error, devuelve 0 o un valor que indique fallo
        SELECT 0 AS Resultado;
    END CATCH
END