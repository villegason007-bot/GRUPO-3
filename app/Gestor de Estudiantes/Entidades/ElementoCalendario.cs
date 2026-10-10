namespace Gestor_de_Estudiantes.Entidades
{
    // Fila del calendario de una comisión: une clases y trabajos por fecha
    // para cumplir con H2 (Escenario 3: consultar el calendario).
    public class ElementoCalendario
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;   // "Clase" | "Trabajo"
        public string Detalle { get; set; } = string.Empty; // estado de la clase o título del trabajo
    }
}
