using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de cálculo de riesgo (H6/H8): arma la lista de
    /// resultados de una comisión para el panel. La configuración de
    /// parámetros (H7) vive en <see cref="ParametrosRiesgoService"/>.
    /// </summary>
    public class RiesgoService : IRiesgoService
    {
        private readonly IRiesgoRepository _riesgos;
        private readonly IEstudianteRepository _estudiantes;
        private readonly ICalculadoraDeRiesgo _calculadora;
        private readonly IContactoRepository _contactos;

        /// <summary>Crea el servicio con los repositorios de riesgo, estudiantes, contactos y la calculadora.</summary>
        public RiesgoService(
            IRiesgoRepository riesgos,
            IEstudianteRepository estudiantes,
            ICalculadoraDeRiesgo calculadora,
            IContactoRepository contactos)
        {
            _riesgos = riesgos;
            _estudiantes = estudiantes;
            _calculadora = calculadora;
            _contactos = contactos;
        }

        /// <summary>
        /// Calcula el riesgo de todos los estudiantes activos de la
        /// comisión (H8): recorre la nómina y, por cada uno, delega en
        /// <see cref="CalcularRiesgoDe(Estudiante, string, double, int, int)"/>
        /// y agrega la fecha del último contacto (H11).
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

            // H11: último contacto por estudiante, para el panel.
            var ultimosContactos =
                _contactos.UltimoContactoPorEstudiante(codigoComision);

            var resultados = new List<ResultadoRiesgo>();

            foreach (var estudiante in
                     _estudiantes.ListarEstudiantes(codigoComision))
            {
                var resultado = CalcularRiesgoDe(
                    estudiante,
                    codigoComision,
                    umbral,
                    maxFaltas,
                    maxTrabajos);

                resultado.FechaUltimoContacto =
                    ultimosContactos.TryGetValue(estudiante.Id, out var contacto)
                        ? contacto.Fecha
                        : null;

                resultados.Add(resultado);
            }

            return resultados;
        }

        /// <summary>
        /// Calcula el riesgo individual de un estudiante por su legajo
        /// (H13), incluyendo su último contacto (H11).
        /// </summary>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        public ResultadoRiesgo CalcularRiesgoDe(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();

            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");

            var estudiante = _estudiantes.BuscarEstudiante(legajo)
                ?? throw new InvalidOperationException(
                    $"No existe el estudiante {legajo}.");

            var resultado = CalcularRiesgoDe(
                estudiante,
                estudiante.CodigoComision,
                _riesgos.ObtenerUmbral(),
                _riesgos.ObtenerMaxFaltasPermitidas(),
                _riesgos.ObtenerMaxTrabajosNoEntregadosPermitidos());

            var contacto = _contactos.UltimoContacto(estudiante.Id);
            resultado.FechaUltimoContacto = contacto?.Fecha;

            return resultado;
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
