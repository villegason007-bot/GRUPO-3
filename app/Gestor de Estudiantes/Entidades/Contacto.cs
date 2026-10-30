namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>Medio por el cual el tutor se comunicó con el estudiante (H10).</summary>
    public enum MedioContacto
    {
        Llamada,
        Mensaje,
    }

    /// <summary>Resultado del contacto con el estudiante (H10).</summary>
    public enum ResultadoContacto
    {
        Contesto,
        QuedoEnVeremos,
        SinRespuesta,
    }

    /// <summary>
    /// Registro de que el tutor contactó a un estudiante (H10): se guarda
    /// con la fecha del día, el medio usado y el resultado obtenido.
    /// </summary>
    public class Contacto
    {
        /// <summary>Identificador único del contacto.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Estudiante contactado.</summary>
        public Guid EstudianteId { get; set; }

        /// <summary>Fecha del contacto (se registra el día que se hace).</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Medio utilizado (llamada o mensaje).</summary>
        public MedioContacto Medio { get; set; }

        /// <summary>Resultado obtenido (contestó, quedó en veremos o sin respuesta).</summary>
        public ResultadoContacto Resultado { get; set; }
    }
}
