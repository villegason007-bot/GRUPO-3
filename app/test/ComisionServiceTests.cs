using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Validaciones de alta de comisiones (QA002): '1k1' no debe chocar
    /// con '1K1' (COLLATE NOCASE).
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class ComisionServiceTests : RepositorioTestBase
    {
        private readonly ComisionService _servicio =
            new(new ComisionRepository());

        [TestMethod]
        public void Given_Comision2K2_When_Crear2k2_Then_LanzaInvalidOperationException()
        {
            _servicio.AgregarComision("2K2");

            Assert.Throws<InvalidOperationException>(() =>
                _servicio.AgregarComision("2k2"));
        }

        [TestMethod]
        public void Given_Comision2K2_When_ExisteComisionCon2k2_Then_DevuelveTrue()
        {
            _servicio.AgregarComision("2K2");

            Assert.IsTrue(new ComisionRepository().ExisteComision("2k2"));
        }

        [TestMethod]
        public void Given_CodigoVacio_When_AgregarComision_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _servicio.AgregarComision("  "));
        }
    }
}
