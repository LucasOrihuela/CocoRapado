CREATE PROCEDURE sp_ObtenerProfesionalesPorServicio
    @ServicioId INT,
	@SucursalId INT
AS
BEGIN
    SELECT
		u.id as id,
		id_perfil as IdPerfil,
		correo as Correo,
		imagen as Imagen,
		nombre as Nombre,
		apellido as Apellido,
		telefono as Telefono,
		fecha_nacimiento as FechaNacimiento
    FROM usuarios u
		INNER JOIN perfiles p ON u.id_perfil = p.id
		INNER JOIN profesional_x_sucursal pxs ON pxs.id_profesional = u.id
		INNER JOIN servicio_x_profesional sp ON u.id = sp.id_profesional
    WHERE sp.id_servicio = @ServicioId and pxs.id_sucursal = @SucursalId
		AND p.rol = 'Profesional'
END