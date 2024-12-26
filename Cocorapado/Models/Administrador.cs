namespace Cocorapado.Models
{
    public class Administrador : Usuario
    {
        public SucursalDTO SucursalDTO { get; set; } = new SucursalDTO();
    }
}
