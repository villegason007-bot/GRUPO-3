using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de historial de entregas del estudiante (H13): estado de
    /// entrega de cada trabajo de su comisión.
    /// </summary>
    public class HistorialEntregasService : IHistorialEntregasService
    {
        private readonly IEstudianteRepository _estudiantes;
        private readonly IEntregaRepository _entregas;

        /// <summary>Crea el servicio con los repositorios de estudiantes y entregas.</summary>
        public HistorialEntregasService(
            IEstudianteRepository estudiantes,
            IEntregaRepository entregas)
        {
            _estudiantes = estudiantes;
            _entregas = entregas;
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
