using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada del calendario de clases (H2): listado de
    /// clases, alta y marca de estado hábil/no hábil.
    /// </summary>
    public interface IClaseService
    {
        /// <summary>Devuelve las clases de la comisión ordenadas por fecha.</summary>
        /// <param name="codigoComision">Código de la comisión (debe existir).</param>
        List<Clase> ListarClases(string codigoComision);

        /// <summary>
        /// Agrega una clase a la comisión, nacida como hábil (H2).
        /// </summary>
        /// <param name="codigoComision">Comisión a la que pertenece la clase.</param>
        /// <param name="fecha">Fecha del encuentro (obligatoria; se guarda sin hora).</param>
        /// <returns>La clase creada.</returns>
        /// <exception cref="ArgumentException">Si falta la fecha.</exception>
        /// <exception cref="InvalidOperationException">Si la comisión no existe.</exception>
        Clase AgregarClase(string codigoComision, DateTime fecha);

        /// <summary>
        /// Marca una clase como hábil o no hábil (feriado/suspensión, H2).
        /// </summary>
        /// <param name="claseId">Clase a actualizar.</param>
        /// <param name="estado">Nuevo estado.</param>
        /// <returns>true si se actualizó; false si la clase no existe.</returns>
        bool MarcarEstado(Guid claseId, EstadoClase estado);
    }
}
