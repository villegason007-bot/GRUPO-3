using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class InscripcionMateriaService : IInscripcionMateriaService
    {
        private readonly IInscripcionMateriaRepository _repository;

        public InscripcionMateriaService(IInscripcionMateriaRepository repository)
        {
            _repository = repository;
        }

        public List<Materia> ListarMateriasDeEstudiante(Guid estudianteId)
        {
            return _repository.ListarMateriasDeEstudiante(estudianteId);
        }

        public List<Estudiante> ListarEstudiantesDeMateria(
            Guid materiaId,
            string? codigoComision = null)
        {
            return _repository.ListarEstudiantesDeMateria(materiaId, codigoComision);
        }

        public void InscribirEstudiante(Guid estudianteId, Guid materiaId)
        {
            if (estudianteId == Guid.Empty)
                throw new ArgumentException("Debe seleccionar un estudiante.");

            if (materiaId == Guid.Empty)
                throw new ArgumentException("Debe seleccionar una materia.");

            if (!_repository.PuedeInscribir(estudianteId, materiaId))
            {
                throw new InvalidOperationException(
                    "El estudiante no puede inscribirse en esta materia. " +
                    "La materia debe estar asignada a su comisión.");
            }

            if (_repository.EstaInscripto(estudianteId, materiaId))
            {
                throw new InvalidOperationException(
                    "El estudiante ya está inscripto en esta materia.");
            }

            if (!_repository.Inscribir(estudianteId, materiaId))
                throw new InvalidOperationException("No se pudo realizar la inscripción.");
        }

        public void DesinscribirEstudiante(Guid estudianteId, Guid materiaId)
        {
            if (!_repository.Desinscribir(estudianteId, materiaId))
            {
                throw new InvalidOperationException(
                    "No se encontró la inscripción que se desea eliminar.");
            }
        }

        public bool EstaInscripto(Guid estudianteId, Guid materiaId)
        {
            return _repository.EstaInscripto(estudianteId, materiaId);
        }
    }
}