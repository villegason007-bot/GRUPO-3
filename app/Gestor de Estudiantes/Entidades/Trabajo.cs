namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Trabajo práctico de una comisión con su fecha de entrega (H5).
    /// </summary>
    public class Trabajo
    {
        /// <summary>Identificador único del trabajo.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Título del trabajo.</summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Fecha límite de entrega; a partir de ella el trabajo se considera vencido para el riesgo.</summary>
        public DateTime FechaDeEntrega { get; set; }
    }
}
