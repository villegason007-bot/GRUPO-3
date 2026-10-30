using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a los registros de asistencia (H3/H4).
    /// </summary>
    public interface IAsistenciaRepository
    {
        /// <summary>Inserta una asistencia nueva. Lanza <see cref="InvalidOperationException"/> si la clase no es hábil.</summary>
        /// <param name="asistencia">Registro a crear (clase, estudiante y condición).</param>
        void RegistrarAsistencia(Asistencia asistencia);

        /// <summary>Cambia la condición de un registro existente de la clase.</summary>
        /// <param name="claseId">Clase del registro.</param>
        /// <param name="estudianteId">Estudiante del registro.</param>
        /// <param name="condicion">Nueva condición (Presente | Ausente).</param>
        /// <returns>true si había registro y se actualizó; false si no existía.</returns>
        bool ActualizarCondicion(Guid claseId, Guid estudianteId, Condicion condicion);

        /// <summary>Devuelve todas las asistencias de una clase, ordenadas por estudiante.</summary>
        /// <param name="claseId">Clase a consultar.</param>
        List<Asistencia> ListarPorClase(Guid claseId);

        /// <summary>Devuelve todas las asistencias de un estudiante, ordenadas por clase.</summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        List<Asistencia> ListarPorEstudiante(Guid estudianteId);

        /// <summary>
        /// Devuelve el historial de asistencia del estudiante (H12): la
        /// fecha de cada clase con registro y la condición (presente o
        /// ausente), ordenado por fecha.
        /// </summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        List<(DateTime Fecha, Condicion Condicion)> ListarHistorialDeEstudiante(Guid estudianteId);
    }
}
