namespace Gestor_de_Estudiantes.Entidades
{
    public class Trabajo
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaDeEntrega { get; set; }
    }
}
