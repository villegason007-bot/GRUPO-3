using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IEntregaRepository
    {
        void RegistrarEntrega(Entrega entrega);
        bool ActualizarEntrega(Guid trabajoId, Guid estudianteId, bool entregado, DateTime? fecha);
        List<Entrega> ListarPorTrabajo(Guid trabajoId);
        List<Entrega> ListarPorEstudiante(Guid estudianteId);
    }
}
