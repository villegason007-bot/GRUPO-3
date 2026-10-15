
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class EntregaService
    {
        private readonly IEntregaRepository _entregas;

        public EntregaService(IEntregaRepository entregas)
        {
            _entregas = entregas;
        }

        public List<Entrega> ListarPorTrabajo(Guid trabajoId)
        {
            return _entregas.ListarPorTrabajo(trabajoId);
        }

        public void GuardarEntrega(
            Guid trabajoId,
            Guid estudianteId,
            bool entregado,
            DateTime? fecha)
        {
            var existentes = _entregas.ListarPorTrabajo(trabajoId);

            var entrega = existentes.FirstOrDefault(e =>
                e.EstudianteId == estudianteId);

            if (entrega != null)
            {
                _entregas.ActualizarEntrega(
                    trabajoId,
                    estudianteId,
                    entregado,
                    entregado ? fecha : null);

                return;
            }

            _entregas.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = entregado,
                Fecha = entregado ? fecha : null
            });
        }
    }
}
