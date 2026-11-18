using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de historial de asistencia del estudiante (H12): fecha de
    /// cada clase con registro y su condición.
    /// </summary>
    public class HistorialAsistenciaService : IHistorialAsistenciaService
    {
        private readonly IEstudianteRepository _estudiantes;
        private readonly IAsistenciaRepository _asistencias;

        /// <summary>Crea el servicio con los repositorios de estudiantes y asistencias.</summary>
        public HistorialAsistenciaService(
            IEstudianteRepository estudiantes,
            IAsistenciaRepository asistencias)
        {
            _estudiantes = estudiantes;
            _asistencias = asistencias;
        }

        /// <summary>Devuelve el historial de asistencia del estudiante (H12); vacío si no tiene registros.</summary>
        public List<RegistroDeAsistencia> HistorialAsistencia(string legajo)
        {
            var estudiante = BuscarEstudiante(legajo);

            return _asistencias
                .ListarHistorialDeEstudiante(estudiante.Id)
                .Select(registro => new RegistroDeAsistencia
                {
                    Fecha = registro.Fecha,
                    Condicion = registro.Condicion,
                })
                .ToList();
        }

        /// <summary>Valida el legajo y devuelve el estudiante activo correspondiente.</summary>
        private Estudiante BuscarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();

            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");

            return _estudiantes.BuscarEstudiante(legajo)
                ?? throw new InvalidOperationException(
                    $"No existe el estudiante {legajo}.");
        }
    }
}
