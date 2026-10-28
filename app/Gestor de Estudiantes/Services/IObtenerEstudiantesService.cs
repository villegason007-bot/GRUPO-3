using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato del MOCK de la nómina de estudiantes: devuelve datos fijos
    /// de prueba sin tocar la base. Solo para probar pantallas.
    /// </summary>
    internal interface IObtenerEstudiantesService
    {
        /// <summary>Devuelve tres estudiantes de prueba fijos.</summary>
        List<Estudiante> ObtenerEstudiantes();
    }
}
