using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de historiales del estudiante (H12/H13):
    /// asistencia por clase y estado de entrega de cada trabajo.
    /// </summary>
    public interface IHistorialService
    {
        /// <summary>
        /// Devuelve el historial de asistencia del estudiante (H12):
        /// fechas de las clases con registro y condición de cada una.
        /// Vacío si no tiene asistencias registradas.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        List<RegistroDeAsistencia> HistorialAsistencia(string legajo);

        /// <summary>
        /// Devuelve el historial de entregas del estudiante (H13):
        /// trabajos de la comisión con su estado de entrega. Vacío si no
        /// hay trabajos cargados.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        List<RegistroDeEntrega> HistorialEntregas(string legajo);
    }

    /// <summary>Una clase con la condición de asistencia del estudiante (H12).</summary>
    public class RegistroDeAsistencia
    {
        /// <summary>Fecha de la clase.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Condición registrada ese día (presente o ausente).</summary>
        public Condicion Condicion { get; set; }

        /// <summary>true si asistió a esa clase.</summary>
        public bool EstaPresente => Condicion == Condicion.Presente;
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
