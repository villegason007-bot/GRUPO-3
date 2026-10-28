using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de asistencia (H3/H4): listado y guardado de
    /// la asistencia de una clase, con semántica de alta o corrección.
    /// </summary>
    public interface IAsistenciaService
    {
        /// <summary>Devuelve las asistencias ya cargadas de una clase.</summary>
        /// <param name="claseId">Clase a consultar.</param>
        List<Asistencia> ListarPorClase(Guid claseId);

        /// <summary>
        /// Guarda la condición de un estudiante en una clase: si ya existe
        /// su registro lo corrige (H4); si no, lo crea (H3).
        /// </summary>
        /// <param name="claseId">Clase de la que se registra la asistencia.</param>
        /// <param name="estudianteId">Estudiante al que pertenece la condición.</param>
        /// <param name="presente">true si asistió; false si faltó.</param>
        void GuardarAsistencia(Guid claseId, Guid estudianteId, bool presente);
    }
}
