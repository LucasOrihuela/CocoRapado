CREATE PROCEDURE sp_VerificaUsuarioBloqueado
    @id_usuario INT
AS
BEGIN

    DECLARE @bloqueado BIT;

    SELECT @bloqueado = bloqueado FROM usuarios WHERE id = @id_usuario;

    SELECT @bloqueado;

END