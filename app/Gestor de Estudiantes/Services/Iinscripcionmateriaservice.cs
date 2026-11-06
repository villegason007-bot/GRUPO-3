using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>Reglas de negocio de la inscripción de estudiantes a materias.</summary>
    public interface IInscripcionMateriaService
    {
        List<Materia> ListarMateriasDeEstudiante(Guid estudianteId);

        List<Estudiante> ListarEstudiantesDeMateria(
            Guid materiaId,
            string? codigoComision = null);

        void InscribirEstudiante(Guid estudianteId, Guid materiaId);

        void DesinscribirEstudiante(Guid estudianteId, Guid materiaId);

        bool EstaInscripto(Guid estudianteId, Guid materiaId);
    }
}