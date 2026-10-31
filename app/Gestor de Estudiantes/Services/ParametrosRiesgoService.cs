using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de parámetros de riesgo (H7): consulta y guardado de
    /// umbral, máximo de faltas y máximo de trabajos sin entregar.
    /// Solo configura; el cálculo vive en <see cref="RiesgoService"/>.
    /// </summary>
    public class ParametrosRiesgoService : IParametrosRiesgoService
    {
        private readonly IParametrosRiesgoRepository _parametros;

        /// <summary>Crea el servicio con el repositorio de parámetros de riesgo.</summary>
        public ParametrosRiesgoService(IParametrosRiesgoRepository parametros)
        {
            _parametros = parametros;
        }

        /// <summary>Devuelve el umbral porcentual vigente (0-100) guardado en parámetros.</summary>
        public double ObtenerUmbral()
            => _parametros.ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        public void GuardarUmbral(double umbral)
            => _parametros.GuardarUmbral(umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3).</summary>
        public int ObtenerMaxFaltasPermitidas()
            => _parametros.ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxFaltasPermitidas(int cantidad)
            => _parametros.GuardarMaxFaltasPermitidas(cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2).</summary>
        public int ObtenerMaxTrabajosNoEntregadosPermitidos()
            => _parametros.ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad)
            => _parametros.GuardarMaxTrabajosNoEntregadosPermitidos(cantidad);
    }
}
