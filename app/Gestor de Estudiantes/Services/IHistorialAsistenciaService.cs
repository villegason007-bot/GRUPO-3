using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de historial de asistencia del estudiante
    /// (H12): fechas de las clases con registro y condición de cada una.
    /// </summary>
    public interface IHistorialAsistenciaService
    {
        /// <summary>
        /// Devuelve el historial de asistencia del estudiante (H12):
        /// fechas de las clases con registro y condición de cada una.
        /// Vacío si no tiene asistencias registradas.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        List<RegistroDeAsistencia> HistorialAsistencia(string legajo);
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
}
