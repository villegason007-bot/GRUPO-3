using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada del calendario unificado (H2): consulta
    /// clases y trabajos de la comisión unidos por fecha.
    /// </summary>
    public interface ICalendarioService
    {
        /// <summary>
        /// Devuelve el calendario de la comisión: clases y trabajos unidos
        /// por fecha (H2, Escenario 3).
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<ElementoCalendario> VerCalendario(string codigoComision);
    }
}
