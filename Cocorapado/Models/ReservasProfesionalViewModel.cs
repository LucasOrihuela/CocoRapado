namespace Cocorapado.Models
{
    public class ReservasProfesionalViewModel
    {
        public Turno Reservas { get; set; }
        public Usuario Usuario { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;

        public ReservasProfesionalViewModel()
        {
            Reservas = new Turno();
            Usuario = new Usuario();
        }
    }
}
