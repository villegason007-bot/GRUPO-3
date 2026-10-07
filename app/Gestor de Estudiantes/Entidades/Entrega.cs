namespace Gestor_de_Estudiantes.Entidades
{
    public class Entrega
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TrabajoId { get; set; }
        public Guid EstudianteId { get; set; }
        public bool Entregado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
