using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de asistencia (H3/H4): lista y guarda la asistencia de una
    /// clase con la semántica de alta o corrección (si ya existe el
    /// registro del estudiante, actualiza su condición).
    /// </summary>
    public class AsistenciaService
    {
        private readonly IAsistenciaRepository _asistencias;

        /// <summary>Crea el servicio con su repositorio de asistencias.</summary>
        public AsistenciaService(IAsistenciaRepository asistencias)
        {
            _asistencias = asistencias;
        }

        /// <summary>Devuelve las asistencias ya cargadas de una clase.</summary>
        /// <param name="claseId">Clase a consultar.</param>
        public List<Asistencia> ListarPorClase(Guid claseId)
        {
            return _asistencias.ListarPorClase(claseId);
        }

        /// <summary>
        /// Guarda la condición de un estudiante en una clase: si ya existe
        /// su registro lo corrige (H4); si no, lo crea (H3).
        /// </summary>
        /// <param name="claseId">Clase de la que se registra la asistencia.</param>
        /// <param name="estudianteId">Estudiante al que pertenece la condición.</param>
        /// <param name="presente">true si asistió; false si faltó.</param>
        /// <exception cref="InvalidOperationException">Si la clase no es hábil (feriado o suspensión).</exception>
        public void GuardarAsistencia(
            Guid claseId,
            Guid estudianteId,
            bool presente)
        {
            var condicion = presente
                ? Condicion.Presente
                : Condicion.Ausente;

            var existentes = _asistencias.ListarPorClase(claseId);

            var asistencia = existentes.FirstOrDefault(a =>
                a.EstudianteId == estudianteId);

            if (asistencia != null)
            {
                _asistencias.ActualizarCondicion(
                    claseId,
                    estudianteId,
                    condicion);

                return;
            }

            _asistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = condicion
            });
        }
    }
}
