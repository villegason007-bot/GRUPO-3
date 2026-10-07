using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class EstudianteService
    {
        private readonly IEstudianteRepository _estudiantes;

        public EstudianteService(IEstudianteRepository estudiantes)
        {
            _estudiantes = estudiantes;
        }

        public List<Estudiante> ListarEstudiantes(string codigoComision)
            => _estudiantes.ListarEstudiantes(codigoComision);

        public Estudiante? BuscarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();
            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");
            return _estudiantes.BuscarEstudiante(legajo);
        }

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

        public bool EliminarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();
            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");
            return _estudiantes.EliminarEstudiante(legajo);
        }
    }
}
