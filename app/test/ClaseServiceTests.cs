using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Validaciones de alta de clases (QA002): la misma comisión no puede
    /// tener dos clases en la misma fecha.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class ClaseServiceTests : RepositorioTestBase
    {
        private readonly ClaseService _servicio =
            new(new ClaseRepository(), new ComisionRepository());

        [TestMethod]
        public void Given_FechaYaRegistrada_When_AgregarClase_Then_LanzaInvalidOperationException()
        {
            var fecha = new DateTime(2026, 11, 10);
            _servicio.AgregarClase("1K1", fecha);

            var excepcion = Assert.Throws<InvalidOperationException>(() =>
                _servicio.AgregarClase("1K1", fecha));

            StringAssert.Contains(excepcion.Message, "10/11/2026");
        }

        [TestMethod]
        public void Given_MismaFechaPeroHoraDistinta_When_AgregarClase_Then_LanzaInvalidOperationException()
        {
            _servicio.AgregarClase("1K1", new DateTime(2026, 11, 10, 8, 0, 0));

            Assert.Throws<InvalidOperationException>(() =>
                _servicio.AgregarClase("1K1", new DateTime(2026, 11, 10, 18, 30, 0)));
        }

        [TestMethod]
        public void Given_FechaDistinta_When_AgregarClase_Then_SeCrea()
        {
            _servicio.AgregarClase("1K1", new DateTime(2026, 11, 10));

            var clase = _servicio.AgregarClase("1K1", new DateTime(2026, 11, 17));

            Assert.AreEqual(new DateTime(2026, 11, 17), clase.Fecha);
        }
    }
}
