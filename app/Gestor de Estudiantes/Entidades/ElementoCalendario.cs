namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Fila del calendario de una comisión: une clases y trabajos por fecha
    /// para cumplir con H2 (Escenario 3: consultar el calendario).
    /// </summary>
    public class ElementoCalendario
    {
        /// <summary>Fecha del evento (de la clase o de entrega del trabajo).</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Tipo de evento: «Clase» o «Trabajo».</summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Detalle a mostrar: estado de la clase (Hábil/NoHábil) o título del trabajo.</summary>
        public string Detalle { get; set; } = string.Empty;
    }
}
