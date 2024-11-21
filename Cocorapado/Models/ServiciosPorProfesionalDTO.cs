namespace Cocorapado.Models
{
    public class ServiciosPorProfesionalDTO
    {
        public int IdServicio { get; set; }
        public int IdProfesional { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string ServicioNombre { get; set; } = string.Empty;
    }
}
