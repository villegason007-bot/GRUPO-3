using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IAsistenciaRepository
    {
        void RegistrarAsistencia(Asistencia asistencia);
        bool ActualizarCondicion(Guid claseId, Guid estudianteId, Condicion condicion);
        List<Asistencia> ListarPorClase(Guid claseId);
        List<Asistencia> ListarPorEstudiante(Guid estudianteId);
    }
}
