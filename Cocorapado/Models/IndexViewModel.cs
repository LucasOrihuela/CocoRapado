namespace Cocorapado.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Sucursal> Sucursales { get; set; }
        public IEnumerable<Usuario> Profesionales { get; set; }
        public List<int> ProfesionalesFavoritos { get; set; }
        public IndexViewModel()
        {
            Sucursales = new List<Sucursal>();
            Profesionales = new List<Usuario>();
            ProfesionalesFavoritos = new List<int>();   
        }
    }
}
