using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Cocorapado.Models
{
    public class Usuario : IdentityUser
    {
        public int IdPerfil { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Perfil Perfil { get; set; } = new Perfil();

        public bool EstaLogueado { get; set; } = false;

    }
}
