using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de entregas (H5): listado y guardado de si
    /// un estudiante entregó un trabajo, con semántica de alta o corrección.
    /// </summary>
    public interface IEntregaService
    {
        /// <summary>Devuelve los registros de entrega de un trabajo.</summary>
        /// <param name="trabajoId">Trabajo a consultar.</param>
        List<Entrega> ListarPorTrabajo(Guid trabajoId);

        /// <summary>
        /// Guarda la entrega de un estudiante para un trabajo: si ya existe
        /// su registro lo corrige; si no, lo crea. La fecha solo se
        /// conserva cuando se entregó.
        /// </summary>
        /// <param name="trabajoId">Trabajo al que corresponde la entrega.</param>
        /// <param name="estudianteId">Estudiante que entregó (o no).</param>
        /// <param name="entregado">true si entregó; false si no entregó.</param>
        /// <param name="fecha">Fecha de la entrega; se ignora (null) si no entregó.</param>
        void GuardarEntrega(Guid trabajoId, Guid estudianteId, bool entregado, DateTime? fecha);
    }
}
