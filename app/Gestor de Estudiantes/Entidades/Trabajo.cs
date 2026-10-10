namespace Gestor_de_Estudiantes.Entidades
{
    public class Trabajo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaDeEntrega { get; set; }
    }
}
