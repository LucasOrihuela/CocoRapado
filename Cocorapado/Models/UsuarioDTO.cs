
using System.Security.Cryptography.X509Certificates;

namespace Cocorapado.Models
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public int IdPerfil { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public bool EstaLogueado { get; set; } = false;
    }
}
