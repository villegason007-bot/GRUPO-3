using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso al calendario unificado de una comisión:
    /// clases y trabajos unidos por fecha (H2, Escenario 3).
    /// </summary>
    public interface ICalendarioRepository
    {
        /// <summary>Devuelve clases y trabajos de la comisión unidos por fecha.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<ElementoCalendario> VerCalendario(string codigoComision);
    }
}
