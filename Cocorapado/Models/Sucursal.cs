using System.Collections.Generic;

namespace Cocorapado.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Localidad { get; set; }
        public string? Imagen { get; set; }
        public string? Telefono { get; set; }
        public int PrecioAbono { get; set; }
        public List<Horario> Horarios { get; set; } = new List<Horario>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
