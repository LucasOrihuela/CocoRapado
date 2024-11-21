CREATE PROCEDURE sp_CrearServicioPorProfesional
    @id_servicio INT,
    @id_profesional INT
AS
BEGIN
    BEGIN TRY
        -- Realiza el insert
        INSERT INTO servicio_x_profesional(id_servicio, id_profesional)
        VALUES (@id_servicio, @id_profesional);

        -- Si la inserción fue exitosa, devuelve 1
        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        -- En caso de error, devuelve 0 o un valor que indique fallo
        SELECT 0 AS Resultado;
    END CATCH
END