using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de trabajos prácticos (H5): listado de
    /// trabajos de la comisión y alta con su fecha de entrega.
    /// </summary>
    public interface ITrabajoService
    {
        /// <summary>Devuelve los trabajos de la comisión ordenados por fecha de entrega.</summary>
        /// <param name="codigoComision">Código de la comisión (debe existir).</param>
        List<Trabajo> ListarTrabajos(string codigoComision);

        /// <summary>
        /// Agrega un trabajo a la comisión (H5).
        /// </summary>
        /// <param name="codigoComision">Comisión a la que pertenece el trabajo.</param>
        /// <param name="titulo">Título del trabajo (obligatorio).</param>
        /// <param name="fechaDeEntrega">Fecha límite de entrega (obligatoria; se guarda sin hora).</param>
        /// <returns>El trabajo creado.</returns>
        /// <exception cref="ArgumentException">Si falta el título o la fecha.</exception>
        /// <exception cref="InvalidOperationException">Si la comisión no existe.</exception>
        Trabajo AgregarTrabajo(string codigoComision, string titulo, DateTime fechaDeEntrega);
    }
}
