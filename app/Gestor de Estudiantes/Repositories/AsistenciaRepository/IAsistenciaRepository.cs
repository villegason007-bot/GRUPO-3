using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IAsistenciaRepository
    {
        void RegistrarAsistencia(Asistencia asistencia);
        bool ActualizarCondicion(long claseId, long estudianteId, Condicion condicion);
        List<Asistencia> ListarPorClase(long claseId);
        List<Asistencia> ListarPorEstudiante(long estudianteId);
    }
}
