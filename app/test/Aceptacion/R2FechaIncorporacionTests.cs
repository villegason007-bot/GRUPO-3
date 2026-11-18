using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// R2 · Incorporación tardía: validación de la fecha de incorporación
    /// al dar de alta un estudiante.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class R2FechaIncorporacionTests : RepositorioTestBase
    {
        private readonly EstudianteService _servicio =
            new(new EstudianteRepository());

        private static string Fecha(DateTime f) => f.ToString("yyyy-MM-dd");

        [TestMethod]
        public void Given_FechaDeAyer_When_AgregarEstudiante_Then_SeGuardaLaFecha()
        {
            var ayer = Fecha(DateTime.Today.AddDays(-1));

            _servicio.AgregarEstudiante("9001", "Ana", "1K1", null, ayer);

            Assert.AreEqual(ayer, _servicio.BuscarEstudiante("9001")!.FechaIncorporacion);
        }

        [TestMethod]
        public void Given_FechaDeHoy_When_AgregarEstudiante_Then_SeAcepta()
        {
            var hoy = Fecha(DateTime.Today);

            _servicio.AgregarEstudiante("9001", "Ana", "1K1", null, hoy);

            Assert.AreEqual(hoy, _servicio.BuscarEstudiante("9001")!.FechaIncorporacion);
        }

        [TestMethod]
        public void Given_FechaPosteriorAHoy_When_AgregarEstudiante_Then_LanzaArgumentException()
        {
            var manana = Fecha(DateTime.Today.AddDays(1));

            var excepcion = Assert.Throws<ArgumentException>(() =>
                _servicio.AgregarEstudiante("9001", "Ana", "1K1", null, manana));

            StringAssert.Contains(excepcion.Message, "posterior a hoy");
            Assert.IsNull(_servicio.BuscarEstudiante("9001"));
        }

        [TestMethod]
        public void Given_TextoQueNoEsUnaFecha_When_AgregarEstudiante_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.AgregarEstudiante("9001", "Ana", "1K1", null, "mañana"));
        }

        [TestMethod]
        public void Given_SinFecha_When_AgregarEstudiante_Then_QuedaSinFecha()
        {
            _servicio.AgregarEstudiante("9001", "Ana", "1K1");

            Assert.IsNull(_servicio.BuscarEstudiante("9001")!.FechaIncorporacion);
        }
    }
}