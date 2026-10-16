namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Comisión o grupo de estudiantes que comparte calendario, asistencias
    /// y trabajos. Es el contexto con el que se agrupan casi todas las
    /// consultas (se la identifica por su <see cref="Codigo"/>).
    /// </summary>
    public class Comision
    {
        /// <summary>Identificador único de la comisión.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Código visible de la comisión (por ejemplo «1K1»), único entre comisiones.</summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>Semanas del cuatrimestre (default 16 según calendario del curso).</summary>
        public int Semanas { get; set; } = 16;
    }
}
