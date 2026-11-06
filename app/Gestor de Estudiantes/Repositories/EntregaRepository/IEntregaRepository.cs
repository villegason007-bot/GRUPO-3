using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a los registros de entrega de trabajos (H5).
    /// </summary>
    public interface IEntregaRepository
    {
        /// <summary>Inserta un registro de entrega nuevo.</summary>
        /// <param name="entrega">Registro a crear (trabajo, estudiante, entregado y fecha).</param>
        void RegistrarEntrega(Entrega entrega);

        /// <summary>Actualiza si un estudiante entregó un trabajo y su fecha.</summary>
        /// <param name="trabajoId">Trabajo del registro.</param>
        /// <param name="estudianteId">Estudiante del registro.</param>
        /// <param name="entregado">Nuevo valor de entregado.</param>
        /// <param name="fecha">Fecha de entrega; null si no entregó.</param>
        /// <returns>true si había registro y se actualizó.</returns>
        bool ActualizarEntrega(Guid trabajoId, Guid estudianteId, bool entregado, DateTime? fecha);

        /// <summary>Devuelve los registros de entrega de un trabajo, ordenados por estudiante.</summary>
        /// <param name="trabajoId">Trabajo a consultar.</param>
        List<Entrega> ListarPorTrabajo(Guid trabajoId);

        /// <summary>Devuelve los registros de entrega de un estudiante, ordenados por trabajo.</summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        List<Entrega> ListarPorEstudiante(Guid estudianteId);

        /// <summary>
        /// Devuelve el historial de entregas del estudiante (H13): todos
        /// los trabajos de su comisión con el estado de entrega
        /// correspondiente, ordenados por fecha de entrega.
        /// </summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        List<(string Titulo, DateTime FechaDeEntrega, bool Entregado, DateTime? Fecha)> ListarHistorialDeEstudiante(Guid estudianteId);
    }
}
