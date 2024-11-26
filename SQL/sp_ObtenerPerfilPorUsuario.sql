CREATE PROCEDURE sp_ObtenerPerfilPorUsuario
    @id_usuario INT
AS
BEGIN
    SELECT 
	p.id as Id,
	rol as Rol
	FROM perfiles p
	INNER JOIN usuarios u ON u.id_perfil = p.id
	WHERE u.id = @id_usuario
END