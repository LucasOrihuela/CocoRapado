namespace Cocorapado.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        public string ServicioDescripcion { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
        public int PrecioMin { get; set; }
        public int PrecioMax { get; set; }
        public string? Imagen { get; set; }
        public bool? Checked { get; set; }
    }
}