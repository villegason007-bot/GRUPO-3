using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de entregas (H5): lista y guarda si un estudiante entregó
    /// un trabajo, con semántica de alta o corrección (si ya existe el
    /// registro, lo actualiza).
    /// </summary>
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregas;

        /// <summary>Crea el servicio con su repositorio de entregas.</summary>
        public EntregaService(IEntregaRepository entregas)
        {
            _entregas = entregas;
        }

        /// <summary>Devuelve los registros de entrega de un trabajo.</summary>
        /// <param name="trabajoId">Trabajo a consultar.</param>
        public List<Entrega> ListarPorTrabajo(Guid trabajoId)
        {
            return _entregas.ListarPorTrabajo(trabajoId);
        }

        /// <summary>
        /// Guarda la entrega de un estudiante para un trabajo: si ya existe
        /// su registro lo corrige; si no, lo crea. La fecha solo se
        /// conserva cuando se entregó.
        /// </summary>
        /// <param name="trabajoId">Trabajo al que corresponde la entrega.</param>
        /// <param name="estudianteId">Estudiante que entregó (o no).</param>
        /// <param name="entregado">true si entregó; false si no entregó.</param>
        /// <param name="fecha">Fecha de la entrega; se ignora (null) si no entregó.</param>
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
