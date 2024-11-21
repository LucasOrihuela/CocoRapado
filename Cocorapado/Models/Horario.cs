namespace Cocorapado.Models
{
    public class Horario
    {
        public int IdSucursal { get; set; }
        public string Dia { get; set; } = string.Empty;
        public string? HorarioApertura { get; set; }
        public string? HorarioCierreMediodia { get; set; }
        public string? HorarioAperturaMediodia { get; set; }
        public string? HorarioCierre { get; set; }
    }
}