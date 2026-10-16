namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Entrega (o no entrega) de un trabajo por parte de un estudiante:
    /// la segunda de las dos señales que alimentan el indicador de
    /// riesgo (H5/H6).
    /// </summary>
    public class Entrega
    {
        /// <summary>Identificador único del registro de entrega.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Clave foránea al <see cref="Trabajo"/> al que corresponde.</summary>
        public Guid TrabajoId { get; set; }

        /// <summary>Clave foránea al <see cref="Estudiante"/> al que corresponde.</summary>
        public Guid EstudianteId { get; set; }

        /// <summary>true si el estudiante entregó el trabajo; false si no entregó.</summary>
        public bool Entregado { get; set; }

        /// <summary>Fecha de la entrega; null cuando no se entregó.</summary>
        public DateTime? Fecha { get; set; }
    }
}
