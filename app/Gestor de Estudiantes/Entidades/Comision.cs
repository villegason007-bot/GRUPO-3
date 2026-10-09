namespace Gestor_de_Estudiantes.Entidades
{
    public class Comision
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Codigo { get; set; } = string.Empty;
    }
}
