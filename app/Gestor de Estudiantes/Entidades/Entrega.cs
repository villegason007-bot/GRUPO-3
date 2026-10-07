namespace Gestor_de_Estudiantes.Entidades
{
    public class Entrega
    {
        public long Id { get; set; }
        public long TrabajoId { get; set; }
        public long EstudianteId { get; set; }
        public bool Entregado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
