namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Persona inscripta en una comisión (nómina de estudiantes, H1).
    /// La baja es lógica: el repositorio mantiene la fila en la base y
    /// la oculta filtrando por <c>activo</c>, conservando el historial.
    /// </summary>
    public class Estudiante
    {
        /// <summary>Identificador único (Guid; se guarda como uuid con guiones en la base).</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Legajo del estudiante, único entre estudiantes.</summary>
        public string Legajo { get; set; } = string.Empty;

        /// <summary>Nombre del estudiante.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Apellido del estudiante.</summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto (opcional, nullable).</summary>
        public string? Telefono { get; set; }

        /// <summary>Fecha de incorporación a la comisión, como texto (opcional, nullable).</summary>
        public string? FechaIncorporacion { get; set; }
    }
}
