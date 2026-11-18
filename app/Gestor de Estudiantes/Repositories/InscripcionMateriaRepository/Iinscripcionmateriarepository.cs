using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>Persistencia de la inscripción de estudiantes a materias (N:M).</summary>
    public interface IInscripcionMateriaRepository
    {
        List<Materia> ListarMateriasDeEstudiante(Guid estudianteId);

        List<Estudiante> ListarEstudiantesDeMateria(
            Guid materiaId,
            string? codigoComision = null);

        /// <summary>true si la materia está asignada a la comisión del estudiante.</summary>
        bool PuedeInscribir(Guid estudianteId, Guid materiaId);

        bool EstaInscripto(Guid estudianteId, Guid materiaId);

        bool Inscribir(Guid estudianteId, Guid materiaId);

        bool Desinscribir(Guid estudianteId, Guid materiaId);
    }
}