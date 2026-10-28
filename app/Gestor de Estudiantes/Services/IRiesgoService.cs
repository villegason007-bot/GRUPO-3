namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada del indicador de riesgo (H6/H7/H8):
    /// consulta y guardado de parámetros y cálculo del panel de una
    /// comisión.
    /// </summary>
    public interface IRiesgoService
    {
        /// <summary>Devuelve el umbral porcentual vigente (0-100) guardado en parámetros.</summary>
        double ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        void GuardarUmbral(double umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3, configurable en la UI).</summary>
        int ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxFaltasPermitidas(int cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2, configurable en la UI).</summary>
        int ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad);

        /// <summary>
        /// Calcula el riesgo de todos los estudiantes activos de la
        /// comisión (H8), con el umbral y los límites vigentes.
        /// </summary>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>Un <see cref="ResultadoRiesgo"/> por estudiante, en el orden de la nómina.</returns>
        List<ResultadoRiesgo> CalcularRiesgos(string codigoComision);
    }
}
