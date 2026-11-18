using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Criterios de aceptación de R1 · Ausencias seguidas.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class R1AusenciasSeguidasAcceptanceTests : RepositorioTestBase
    {
        private readonly RiesgoService _servicio =
            new(new RiesgoRepository(), new ParametrosRiesgoRepository(), new EstudianteRepository(), new CalculadoraDeRiesgo(), new ContactoRepository());

        private readonly AsistenciaRepository _asistencias = new();

        private static readonly DateTime Inicio = new(2026, 3, 2);

        /// <summary>Crea 6 clases semanales con asistencia tomada para el estudiante, según el patrón dado.</summary>
        private void CargarAsistencias(Guid estudianteId, params Condicion[] patron)
        {
            for (var i = 0; i < patron.Length; i++)
            {
                var claseId = CrearClaseSiNoExiste(Inicio.AddDays(i * 7));
                _asistencias.RegistrarAsistencia(new Asistencia
                {
                    ClaseId = claseId,
                    EstudianteId = estudianteId,
                    Condicion = patron[i],
                });
            }
        }

        private static Guid CrearClaseSiNoExiste(DateTime fecha)
        {
            var existente = new ClaseRepository().ListarClases("1K1")
                .FirstOrDefault(c => c.Fecha == fecha);
            return existente?.Id ?? CrearClase(fecha);
        }

        private ResultadoRiesgo Riesgo(string legajo) =>
            _servicio.CalcularRiesgos("1K1").Single(r => r.Legajo == legajo);

        [TestMethod]
        public void Given_PresenteEn2PrimerasYAusenteEn4Ultimas_When_CalcularRiesgos_Then_4AusenciasSeguidas()
        {
            var perez = CrearEstudiante("1001", "Perez");
            CargarAsistencias(perez,
                Condicion.Presente, Condicion.Presente,
                Condicion.Ausente, Condicion.Ausente, Condicion.Ausente, Condicion.Ausente);

            var resultado = Riesgo("1001");

            Assert.AreEqual(4, resultado.AusenciasSeguidas);
            Assert.AreEqual("4 ausencias seguidas", resultado.TextoAusenciasSeguidas);
        }

        [TestMethod]
        public void Given_AusenteEnClases3a5YPresenteEnLa6_When_CalcularRiesgos_Then_SinAusenciasSeguidas()
        {
            var gomez = CrearEstudiante("1002", "Gomez");
            CargarAsistencias(gomez,
                Condicion.Presente, Condicion.Presente,
                Condicion.Ausente, Condicion.Ausente, Condicion.Ausente,
                Condicion.Presente);

            var resultado = Riesgo("1002");

            Assert.AreEqual(0, resultado.AusenciasSeguidas);
            Assert.AreEqual("Sin ausencias seguidas", resultado.TextoAusenciasSeguidas);
        }

        [TestMethod]
        public void Given_ClasesFuturasYClasePasadaSinAsistenciaTomada_When_CalcularRiesgos_Then_LaRachaNoCambia()
        {
            var perez = CrearEstudiante("1001", "Perez");
            CargarAsistencias(perez,
                Condicion.Presente, Condicion.Presente,
                Condicion.Ausente, Condicion.Ausente, Condicion.Ausente, Condicion.Ausente);

            // 1 clase pasada sin asistencia tomada (sin registro) ...
            CrearClase(Inicio.AddDays(6 * 7));
            // ... y 2 clases futuras (sin registro).
            CrearClase(DateTime.Today.AddDays(7));
            CrearClase(DateTime.Today.AddDays(14));

            var resultado = Riesgo("1001");

            Assert.AreEqual(4, resultado.AusenciasSeguidas);
        }

        [TestMethod]
        public void Given_ClaseConAsistenciaPendiente_When_CalcularRiesgos_Then_NoSumaNiCortaLaRacha()
        {
            var perez = CrearEstudiante("1001", "Perez");
            CargarAsistencias(perez,
                Condicion.Presente, Condicion.Ausente, Condicion.Ausente, Condicion.Pendiente);

            var resultado = Riesgo("1001");

            Assert.AreEqual(2, resultado.AusenciasSeguidas);
        }

        [TestMethod]
        public void Given_EstudianteSinAsistenciaRegistrada_When_CalcularRiesgos_Then_SinDatos()
        {
            CrearEstudiante("1003", "Diaz");
            CrearClase(Inicio);

            var resultado = Riesgo("1003");

            Assert.IsNull(resultado.AusenciasSeguidas);
            Assert.AreEqual("Sin datos", resultado.TextoAusenciasSeguidas);
        }
    }
}