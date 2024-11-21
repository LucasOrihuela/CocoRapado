namespace Cocorapado.Models
{
    public class Turno
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } // Cambiado a DateTime
        public TimeSpan Hora { get; set; }  // Cambiado a TimeSpan
        public int IdSucursal { get; set; }
        public int IdProfesional { get; set; }
        public int IdCliente { get; set; }
        public int Precio { get; set; }
        public int DuracionMin { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string ProfesionalNombre { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public int Ausente { get; set; } = 0;
        public int AusenteTurnoAnterior { get; set; } = 0;
    }
}