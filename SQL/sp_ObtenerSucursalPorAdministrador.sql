CREATE PROCEDURE sp_ObtenerSucursalPorAdministrador
	@id_admin INT
AS
BEGIN
    SELECT
		axs.id_sucursal as IdSucursal
    FROM usuarios u
		INNER JOIN perfiles p ON u.id_perfil = p.id
		INNER JOIN administrador_x_sucursal axs ON axs.id_admin = u.id
		WHERE p.rol like '%Administrador%' AND u.id = 3
END
