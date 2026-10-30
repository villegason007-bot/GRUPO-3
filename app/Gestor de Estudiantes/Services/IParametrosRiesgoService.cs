namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de parámetros de riesgo (H7): consulta y
    /// guardado de umbral, máximo de faltas y máximo de trabajos sin
    /// entregar. Separa la configuración del cálculo del panel
    /// (<see cref="IRiesgoService"/>).
    /// </summary>
    public interface IParametrosRiesgoService
    {
        /// <summary>Devuelve el umbral porcentual vigente (0-100) guardado en parámetros.</summary>
        double ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        void GuardarUmbral(double umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3).</summary>
        int ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxFaltasPermitidas(int cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2).</summary>
        int ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad);
    }
}
