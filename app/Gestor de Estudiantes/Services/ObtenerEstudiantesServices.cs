using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// MOCK de la nómina de estudiantes: devuelve datos fijos de prueba sin
    /// tocar la base. Solo para probar pantallas; el flujo real (H1) usa
    /// <see cref="EstudianteService"/> con <c>EstudianteRepository</c>.
    /// </summary>
    internal class ObtenerEstudiantesServices : IObtenerEstudiantesService
    {
        /// <summary>Crea el mock.</summary>
        public ObtenerEstudiantesServices()
        {
        }

        /// <summary>Devuelve tres estudiantes de prueba fijos (Juan Pérez, María González, Pedro López).</summary>
        public List<Estudiante> ObtenerEstudiantes()
        {
            return new List<Estudiante>()
            {
                new Estudiante() { Nombre = "Juan", Apellido = "Perez"},
                new Estudiante() { Nombre = "Maria", Apellido = "Gonzalez"},
                new Estudiante() { Nombre = "Pedro", Apellido = "Lopez"},
            };
        }

    }

}
