using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IComisionRepository
    {
        List<Comision> ListarComisiones();
        void AgregarComision(Comision comision);
        bool ExisteComision(string codigo);
    }
}
