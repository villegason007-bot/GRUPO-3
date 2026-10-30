namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de cálculo de riesgo (H6/H8): arma la
    /// lista de resultados de una comisión para el panel. La
    /// configuración de parámetros (H7) vive en
    /// <see cref="IParametrosRiesgoService"/>.
    /// </summary>
    public interface IRiesgoService
    {
        /// <summary>
        /// Calcula el riesgo de todos los estudiantes activos de la
        /// comisión (H8), con el umbral y los límites vigentes.
        /// </summary>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>Un <see cref="ResultadoRiesgo"/> por estudiante, en el orden de la nómina.</returns>
        List<ResultadoRiesgo> CalcularRiesgos(string codigoComision);
    }
}
