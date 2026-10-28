using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de la nómina de estudiantes (H1): listado, búsqueda por
    /// legajo, alta y baja lógica.
    /// </summary>
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudiantes;

        /// <summary>Crea el servicio con su repositorio de estudiantes.</summary>
        public EstudianteService(IEstudianteRepository estudiantes)
        {
            _estudiantes = estudiantes;
        }

        /// <summary>Devuelve los estudiantes activos de una comisión, ordenados por legajo.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        public List<Estudiante> ListarEstudiantes(string codigoComision)
            => _estudiantes.ListarEstudiantes(codigoComision);

        /// <summary>
        /// Busca un estudiante activo por su legajo.
        /// </summary>
        /// <param name="legajo">Legajo a buscar (obligatorio).</param>
        /// <returns>El estudiante, o null si no existe o está dado de baja.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        public Estudiante? BuscarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();
            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");
            return _estudiantes.BuscarEstudiante(legajo);
        }

        /// <summary>
        /// Da de alta un estudiante en la comisión (H1), validando campos
        /// obligatorios y legajo único.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (obligatorio, único).</param>
        /// <param name="nombre">Nombre del estudiante (obligatorio).</param>
        /// <param name="codigoComision">Comisión en la que se incorpora.</param>
        /// <returns>El estudiante creado.</returns>
        /// <exception cref="ArgumentException">Si legajo o nombre están vacíos.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un estudiante con ese legajo.</exception>
        public Estudiante AgregarEstudiante(string legajo, string nombre, string codigoComision)
        {
            legajo = (legajo ?? string.Empty).Trim();
            nombre = (nombre ?? string.Empty).Trim();

            if (legajo.Length == 0 || nombre.Length == 0)
                throw new ArgumentException("Legajo y nombre son obligatorios.");
            if (_estudiantes.BuscarEstudiante(legajo) is not null)
                throw new InvalidOperationException($"Ya existe un estudiante con legajo {legajo}.");

            var estudiante = new Estudiante { Legajo = legajo, Nombre = nombre };
            _estudiantes.AgregarEstudiante(estudiante, codigoComision);
            return estudiante;
        }

        /// <summary>
        /// Da de baja lógica al estudiante: la fila queda en la base y se
        /// oculta de los listados (H1).
        /// </summary>
        /// <param name="legajo">Legajo del estudiante a dar de baja (obligatorio).</param>
        /// <returns>true si se dio de baja; false si no estaba activo.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        public bool EliminarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();
            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");
            return _estudiantes.EliminarEstudiante(legajo);
        }
    }
}
