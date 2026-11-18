using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    /// <summary>
    /// Parámetros configurables del riesgo (H7) sobre
    /// ParametrosRiesgoRepository (SRP: separado de los conteos de
    /// RiesgoRepository).
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class ParametrosRiesgoRepositoryTests : RepositorioTestBase
    {
        private readonly ParametrosRiesgoRepository _repositorio = new();

        [TestMethod]
        public void Given_BDRecienCreada_When_ObtenerUmbral_Then_Es50()
        {
            Assert.AreEqual(50, _repositorio.ObtenerUmbral());
        }

        [TestMethod]
        public void Given_UmbralModificado_When_ObtenerUmbral_Then_DevuelveElNuevo()
        {
            _repositorio.GuardarUmbral(75);

            Assert.AreEqual(75, _repositorio.ObtenerUmbral());
        }

        [TestMethod]
        public void Given_UmbralInvalido_When_GuardarUmbral_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _repositorio.GuardarUmbral(-1));
            Assert.Throws<ArgumentException>(() => _repositorio.GuardarUmbral(101));
        }

        [TestMethod]
        public void Given_MaxFaltasNegativo_When_Guardar_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _repositorio.GuardarMaxFaltasPermitidas(-1));
        }

        [TestMethod]
        public void Given_MaxTrabajosNegativo_When_Guardar_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _repositorio.GuardarMaxTrabajosNoEntregadosPermitidos(-1));
        }
    }
}
