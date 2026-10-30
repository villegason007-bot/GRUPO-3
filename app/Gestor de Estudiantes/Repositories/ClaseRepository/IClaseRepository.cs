using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a las clases del calendario (H2).
    /// </summary>
    public interface IClaseRepository
    {
        /// <summary>Devuelve las clases de la comisión ordenadas por fecha.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Clase> ListarClases(string codigoComision);

        /// <summary>Inserta una clase nueva vinculada a la comisión.</summary>
        /// <param name="clase">Clase a crear (fecha y estado).</param>
        /// <param name="codigoComision">Comisión a la que pertenece.</param>
        void AgregarClase(Clase clase, string codigoComision);

        /// <summary>Actualiza el estado (hábil/no hábil) de una clase.</summary>
        /// <param name="claseId">Clase a actualizar.</param>
        /// <param name="estado">Nuevo estado.</param>
        /// <returns>true si la clase existía y se actualizó.</returns>
        bool MarcarEstado(Guid claseId, EstadoClase estado);

        /// <summary>
        /// Indica si la comisión ya tiene una clase en esa fecha (QA002:
        /// evita registrar dos veces el mismo día).
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        /// <param name="fecha">Fecha a verificar (se compara por día).</param>
        bool ExisteClaseEnFecha(string codigoComision, DateTime fecha);
    }
}
