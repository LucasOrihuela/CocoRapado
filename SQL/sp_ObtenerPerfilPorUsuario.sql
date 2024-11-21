CREATE PROCEDURE sp_ObtenerPerfilPorUsuario
    @id_usuario INT
AS
BEGIN
    SELECT 
	p.id as Id,
	rol as Rol,
	permiso_admin as PermisoAdmin,
	permiso_super_admin as PermisoSuperAdmin,
	permiso_profesional as PermisoProfesional
	FROM perfiles p
	INNER JOIN usuarios u ON u.id_perfil = p.id
	WHERE u.id = @id_usuario
END