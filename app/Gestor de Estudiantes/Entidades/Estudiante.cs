namespace Gestor_de_Estudiantes.Entidades
{
    public class Estudiante
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // uuid
        public string Legajo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? FechaIncorporacion { get; set; }
    }
}
