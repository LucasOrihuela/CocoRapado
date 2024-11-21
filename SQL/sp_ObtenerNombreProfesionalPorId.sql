CREATE PROCEDURE sp_ObtenerNombreProfesionalPorId
    @IdProfesional INT
AS
BEGIN
    SELECT 
        CONCAT(nombre,' ',apellido) as Nombre
    FROM 
        usuarios u
	INNER JOIN perfiles p ON p.id = u.id_perfil
    WHERE 
        u.Id = @IdProfesional and p.rol like 'Profesional'
END