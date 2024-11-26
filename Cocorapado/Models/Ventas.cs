namespace Cocorapado.Models
{
    public class Ventas
    {
        public string Dia { get; set; }  // Puede ser una fecha completa o un día del mes
        public string Hora { get; set; } // Solo necesario para ventas diarias
        public decimal TotalVentas { get; set; }
        public int idSucursal { get; set; }
    }
}
