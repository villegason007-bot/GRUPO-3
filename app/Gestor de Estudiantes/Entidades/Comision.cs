namespace Gestor_de_Estudiantes.Entidades
{
    public class Comision
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Codigo { get; set; } = string.Empty;

        // Semanas del cuatrimestre (default 16 según calendario del curso).
        public int Semanas { get; set; } = 16;
    }
}
