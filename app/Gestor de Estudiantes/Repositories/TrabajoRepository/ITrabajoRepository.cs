using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a los trabajos prácticos de una comisión (H5).
    /// </summary>
    public interface ITrabajoRepository
    {
        /// <summary>Devuelve los trabajos de la comisión ordenados por fecha de entrega.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Trabajo> ListarTrabajos(string codigoComision);

        /// <summary>Inserta un trabajo nuevo vinculado a la comisión.</summary>
        /// <param name="trabajo">Trabajo a crear (título y fecha de entrega).</param>
        /// <param name="codigoComision">Comisión a la que pertenece.</param>
        void AgregarTrabajo(Trabajo trabajo, string codigoComision);
    }
}
