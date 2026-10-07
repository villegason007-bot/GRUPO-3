using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IEntregaRepository
    {
        void RegistrarEntrega(Entrega entrega);
        bool ActualizarEntrega(long trabajoId, long estudianteId, bool entregado, DateTime? fecha);
        List<Entrega> ListarPorTrabajo(long trabajoId);
        List<Entrega> ListarPorEstudiante(long estudianteId);
    }
}
