using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de historiales del estudiante (H12/H13): asistencia por
    /// clase y estado de entrega de cada trabajo de la comisión.
    /// </summary>
    public class HistorialService : IHistorialService
    {
        private readonly IEstudianteRepository _estudiantes;
        private readonly IAsistenciaRepository _asistencias;
        private readonly IEntregaRepository _entregas;

        /// <summary>Crea el servicio con los repositorios de estudiantes, asistencias y entregas.</summary>
        public HistorialService(
            IEstudianteRepository estudiantes,
            IAsistenciaRepository asistencias,
            IEntregaRepository entregas)
        {
            _estudiantes = estudiantes;
            _asistencias = asistencias;
            _entregas = entregas;
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

        /// <summary>Devuelve el historial de entregas del estudiante (H13); vacío si no hay trabajos.</summary>
        public List<RegistroDeEntrega> HistorialEntregas(string legajo)
        {
            var estudiante = BuscarEstudiante(legajo);

            return _entregas
                .ListarHistorialDeEstudiante(estudiante.Id)
                .Select(registro => new RegistroDeEntrega
                {
                    Titulo = registro.Titulo,
                    FechaDeEntrega = registro.FechaDeEntrega,
                    Entregado = registro.Entregado,
                    Fecha = registro.Fecha,
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
