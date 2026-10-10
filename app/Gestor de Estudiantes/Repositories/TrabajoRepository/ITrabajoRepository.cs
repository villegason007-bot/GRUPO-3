using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface ITrabajoRepository
    {
        List<Trabajo> ListarTrabajos(string codigoComision);
        void AgregarTrabajo(Trabajo trabajo, string codigoComision);
    }
}
