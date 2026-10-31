using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ComisionRepositoryTests : RepositorioTestBase
    {
        private readonly ComisionRepository _repositorio = new();

        [TestMethod]
        public void Given_NuevaComision_When_Agregar_Then_SemanasPorDefecto16()
        {
            _repositorio.AgregarComision(new Comision { Codigo = "2K2" });

            var comision = _repositorio.ListarComisiones().Single(c => c.Codigo == "2K2");

            Assert.AreEqual(16, comision.Semanas);
        }

        [TestMethod]
        public void Given_ComisionConSemanas_When_Agregar_Then_LasGuarda()
        {
            _repositorio.AgregarComision(new Comision { Codigo = "3K3", Semanas = 12 });

            var comision = _repositorio.ListarComisiones().Single(c => c.Codigo == "3K3");

            Assert.AreEqual(12, comision.Semanas);
        }

        [TestMethod]
        public void Given_ComisionSemilla_When_Listar_Then_Tiene16Semanas()
        {
            var comision = _repositorio.ListarComisiones().Single(c => c.Codigo == "1K1");

            Assert.AreEqual(16, comision.Semanas);
        }
    }
}
