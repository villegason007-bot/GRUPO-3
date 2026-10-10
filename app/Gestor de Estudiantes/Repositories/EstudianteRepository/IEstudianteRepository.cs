using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IEstudianteRepository
    {
        List<Estudiante> ListarEstudiantes(string codigoComision);
        Estudiante? BuscarEstudiante(string legajo);
        void AgregarEstudiante(Estudiante estudiante, string codigoComision);
        bool EliminarEstudiante(string legajo);
    }
}
