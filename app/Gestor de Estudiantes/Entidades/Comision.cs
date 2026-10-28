namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Comisión o grupo de estudiantes que comparte calendario, asistencias
    /// y trabajos. Es el contexto con el que se agrupan casi todas las
    /// consultas (se la identifica por su <see cref="Codigo"/>).
    /// </summary>
    public class Comision
    {
        /// <summary>Semanas del cuatrimestre por defecto (calendario del curso): única fuente del valor 16.</summary>
        public const int SemanasPorDefecto = 16;

        /// <summary>Identificador único de la comisión.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Código visible de la comisión (por ejemplo «1K1»), único entre comisiones.</summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>Semanas del cuatrimestre (por defecto <see cref="SemanasPorDefecto"/>).</summary>
        public int Semanas { get; set; } = SemanasPorDefecto;
    }
}
