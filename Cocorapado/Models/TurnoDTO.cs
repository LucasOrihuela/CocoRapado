namespace Cocorapado.Models
{
    public class TurnoDTO
    {
        public int Id { get; set; }
        public string? Sucursal { get; set; }
        public string? Profesional { get; set; }
        public List<String> Servicios { get; set; } = new List<String>();
    }
}