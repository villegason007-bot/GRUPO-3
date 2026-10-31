namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de persistencia de los parámetros configurables del
    /// indicador de riesgo (H7): umbral, máximo de faltas y máximo de
    /// trabajos sin entregar. Solo configura; los conteos que alimentan
    /// el cálculo viven en <see cref="IRiesgoRepository"/>.
    /// </summary>
    public interface IParametrosRiesgoRepository
    {
        /// <summary>Devuelve el umbral porcentual vigente (0-100).</summary>
        double ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <param name="umbral">Valor a guardar.</param>
        /// <exception cref="ArgumentException">Si está fuera de 0-100.</exception>
        void GuardarUmbral(double umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3 si no está guardado).</summary>
        int ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas.</summary>
        /// <param name="cantidad">Nuevo máximo (no puede ser negativo).</param>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxFaltasPermitidas(int cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2 si no está guardado).</summary>
        int ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos.</summary>
        /// <param name="cantidad">Nuevo máximo (no puede ser negativo).</param>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad);
    }
}
