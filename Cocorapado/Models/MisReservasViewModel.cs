namespace Cocorapado.Models
{
    public class MisReservasViewModel
    {
        public Turno Reservas { get; set; }
        public Usuario Usuario { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string ProfesionalNombre { get; set; } = string.Empty;

        public MisReservasViewModel()
        {
            Reservas = new Turno();
            Usuario = new Usuario();
        }
    }
}
