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
        private readonly IRiesgoRepository _riesgos;

        /// <summary>Crea el servicio con el repositorio de parámetros de riesgo.</summary>
        public ParametrosRiesgoService(IRiesgoRepository riesgos)
        {
            _riesgos = riesgos;
        }

        /// <summary>Devuelve el umbral porcentual vigente (0-100) guardado en parámetros.</summary>
        public double ObtenerUmbral()
            => _riesgos.ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        public void GuardarUmbral(double umbral)
            => _riesgos.GuardarUmbral(umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3).</summary>
        public int ObtenerMaxFaltasPermitidas()
            => _riesgos.ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxFaltasPermitidas(int cantidad)
            => _riesgos.GuardarMaxFaltasPermitidas(cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2).</summary>
        public int ObtenerMaxTrabajosNoEntregadosPermitidos()
            => _riesgos.ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad)
            => _riesgos.GuardarMaxTrabajosNoEntregadosPermitidos(cantidad);
    }
}
