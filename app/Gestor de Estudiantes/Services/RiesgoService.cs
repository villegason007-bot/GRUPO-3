using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada del indicador de riesgo (H6/H7/H8): consulta y guarda los
    /// parámetros de riesgo y arma la lista de resultados de una comisión
    /// para el panel.
    /// </summary>
    public class RiesgoService : IRiesgoService
    {
        private readonly IRiesgoRepository _riesgos;
        private readonly IEstudianteRepository _estudiantes;
        private readonly ICalculadoraDeRiesgo _calculadora;

        /// <summary>Crea el servicio con los repositorios de riesgo y estudiantes y la calculadora.</summary>
        public RiesgoService(
            IRiesgoRepository riesgos,
            IEstudianteRepository estudiantes,
            ICalculadoraDeRiesgo calculadora)
        {
            _riesgos = riesgos;
            _estudiantes = estudiantes;
            _calculadora = calculadora;
        }

        /// <summary>Devuelve el umbral porcentual vigente (0-100) guardado en parámetros.</summary>
        public double ObtenerUmbral()
            => _riesgos.ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100). Lo usa la consola de desarrollo.</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        public void GuardarUmbral(double umbral)
            => _riesgos.GuardarUmbral(umbral);

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3, configurable en la UI).</summary>
        public int ObtenerMaxFaltasPermitidas()
            => _riesgos.ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxFaltasPermitidas(int cantidad)
            => _riesgos.GuardarMaxFaltasPermitidas(cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2, configurable en la UI).</summary>
        public int ObtenerMaxTrabajosNoEntregadosPermitidos()
            => _riesgos.ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad)
            => _riesgos.GuardarMaxTrabajosNoEntregadosPermitidos(cantidad);

        /// <summary>
        /// Calcula el riesgo de todos los estudiantes activos de la
        /// comisión (H8): recorre la nómina y, por cada uno, delega en
        /// <see cref="CalcularRiesgoDe"/>.
        /// </summary>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>Un <see cref="ResultadoRiesgo"/> por estudiante, en el orden de la nómina.</returns>
        public List<ResultadoRiesgo> CalcularRiesgos(
            string codigoComision)
        {
            var umbral = _riesgos.ObtenerUmbral();
            var maxFaltas = _riesgos.ObtenerMaxFaltasPermitidas();
            var maxTrabajos =
                _riesgos.ObtenerMaxTrabajosNoEntregadosPermitidos();

            var resultados = new List<ResultadoRiesgo>();

            foreach (var estudiante in
                     _estudiantes.ListarEstudiantes(codigoComision))
            {
                resultados.Add(CalcularRiesgoDe(
                    estudiante,
                    codigoComision,
                    umbral,
                    maxFaltas,
                    maxTrabajos));
            }

            return resultados;
        }

        /// <summary>
        /// Cuenta asistencias y trabajos de un estudiante y calcula su
        /// resultado individual, adjuntando los límites configurados.
        /// </summary>
        private ResultadoRiesgo CalcularRiesgoDe(
            Estudiante estudiante,
            string codigoComision,
            double umbral,
            int maxFaltas,
            int maxTrabajos)
        {
            var (clasesHabiles, faltas, conRegistro) =
                _riesgos.ContarAsistencias(
                    estudiante.Id, codigoComision);

            var (vencidos, entregados) =
                _riesgos.ContarTrabajos(
                    estudiante.Id, codigoComision);

            var resultado = _calculadora.Calcular(
                conRegistro ? clasesHabiles : 0,
                faltas,
                vencidos,
                vencidos - entregados,
                umbral);

            resultado.EstudianteId = estudiante.Id;
            resultado.Legajo = estudiante.Legajo;
            resultado.Nombre =
                $"{estudiante.Nombre} {estudiante.Apellido}".Trim();

            resultado.MaxFaltasPermitidas = maxFaltas;
            resultado.MaxTrabajosNoEntregadosPermitidos = maxTrabajos;

            return resultado;
        }
    }
}
