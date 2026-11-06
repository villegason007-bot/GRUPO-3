namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de historial de entregas del estudiante
    /// (H13): trabajos de su comisión con el estado de entrega.
    /// </summary>
    public interface IHistorialEntregasService
    {
        /// <summary>
        /// Devuelve el historial de entregas del estudiante (H13):
        /// trabajos de la comisión con su estado de entrega. Vacío si no
        /// hay trabajos cargados.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        List<RegistroDeEntrega> HistorialEntregas(string legajo);
    }

    /// <summary>Un trabajo con el estado de entrega del estudiante (H13).</summary>
    public class RegistroDeEntrega
    {
        /// <summary>Título del trabajo.</summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Fecha de entrega del trabajo.</summary>
        public DateTime FechaDeEntrega { get; set; }

        /// <summary>true si el estudiante lo entregó.</summary>
        public bool Entregado { get; set; }

        /// <summary>Fecha en que lo entregó; null si no entregó.</summary>
        public DateTime? Fecha { get; set; }

        /// <summary>true si sigue sin entregarlo.</summary>
        public bool SinEntregar => !Entregado;
    }
}
