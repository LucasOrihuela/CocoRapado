namespace Cocorapado.Models
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int PermisoAdmin { get; set; }
        public int PermisoSuperAdmin { get; set; }
        public int PermisoProfesional { get; set; }
    }
}
