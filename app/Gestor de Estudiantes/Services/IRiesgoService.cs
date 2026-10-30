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
        /// comisión (H8), con el umbral y los límites vigentes. Cada
        /// resultado incluye la fecha del último contacto (H11).
        /// </summary>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>Un <see cref="ResultadoRiesgo"/> por estudiante, en el orden de la nómina.</returns>
        List<ResultadoRiesgo> CalcularRiesgos(string codigoComision);

        /// <summary>
        /// Calcula el riesgo individual de un estudiante por su legajo
        /// (H13), con el umbral y los límites vigentes.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir y estar activo).</param>
        /// <returns>El resultado del estudiante; con <c>SinInformacion</c> si no hay datos suficientes (H13, Escenario 3).</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        ResultadoRiesgo CalcularRiesgoDe(string legajo);
    }
}
