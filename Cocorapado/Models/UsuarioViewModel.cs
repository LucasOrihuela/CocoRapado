namespace Cocorapado.Models
{
    public class UsuarioViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public string Clave { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public int IdPerfil { get; set; } = 1;
        public int IdSucursal { get; set; } = 0;
    }
}
