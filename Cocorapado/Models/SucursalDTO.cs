namespace Cocorapado.Models
{
    public class SucursalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Localidad { get; set; }
        public string? Imagen { get; set; }
        public string? Telefono { get; set; }
        public int PrecioAbono { get; set; }
    }
}
