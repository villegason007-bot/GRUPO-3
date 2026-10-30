using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Parámetros de riesgo (H7) separados del cálculo: consulta y
    /// guardado de umbral, máximo de faltas y máximo de trabajos.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class ParametrosRiesgoServiceTests : RepositorioTestBase
    {
        private readonly ParametrosRiesgoService _servicio =
            new(new RiesgoRepository());

        [TestMethod]
        public void Given_BDRecienCreada_When_ObtenerParametros_Then_LosPorDefecto()
        {
            Assert.AreEqual(50, _servicio.ObtenerUmbral());
            Assert.AreEqual(3, _servicio.ObtenerMaxFaltasPermitidas());
            Assert.AreEqual(2, _servicio.ObtenerMaxTrabajosNoEntregadosPermitidos());
        }

        [TestMethod]
        public void Given_UmbralValido_When_Guardar_Then_DevuelveElNuevo()
        {
            _servicio.GuardarUmbral(90);

            Assert.AreEqual(90, _servicio.ObtenerUmbral());
        }

        [TestMethod]
        public void Given_UmbralInvalido_When_Guardar_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _servicio.GuardarUmbral(200));
        }

        [TestMethod]
        public void Given_MaxFaltasNegativo_When_Guardar_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.GuardarMaxFaltasPermitidas(-1));
        }

        [TestMethod]
        public void Given_MaxTrabajosModificado_When_Guardar_Then_DevuelveElNuevo()
        {
            _servicio.GuardarMaxTrabajosNoEntregadosPermitidos(5);

            Assert.AreEqual(5, _servicio.ObtenerMaxTrabajosNoEntregadosPermitidos());
        }
    }
}
