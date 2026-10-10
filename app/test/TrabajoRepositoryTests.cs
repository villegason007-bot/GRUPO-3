using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class TrabajoRepositoryTests : RepositorioTestBase
    {
        private readonly TrabajoRepository _repositorio = new();

        [TestMethod]
        public void Given_NuevoTrabajo_When_AgregarTrabajo_Then_ApareceConTituloYFecha()
        {
            var fecha = new DateTime(2026, 10, 20);

            _repositorio.AgregarTrabajo(new Trabajo { Titulo = "TP1", FechaDeEntrega = fecha }, "1K1");

            var trabajos = _repositorio.ListarTrabajos("1K1");
            Assert.AreEqual(1, trabajos.Count);
            Assert.AreEqual("TP1", trabajos[0].Titulo);
            Assert.AreEqual(fecha, trabajos[0].FechaDeEntrega);
            Assert.AreNotEqual(Guid.Empty, trabajos[0].Id);
        }

        [TestMethod]
        public void Given_DosTrabajos_When_ListarTrabajos_Then_DevuelveOrdenadosPorFechaDeEntrega()
        {
            _repositorio.AgregarTrabajo(new Trabajo { Titulo = "TP2", FechaDeEntrega = new DateTime(2026, 11, 3) }, "1K1");
            _repositorio.AgregarTrabajo(new Trabajo { Titulo = "TP1", FechaDeEntrega = new DateTime(2026, 10, 20) }, "1K1");

            var trabajos = _repositorio.ListarTrabajos("1K1");

            Assert.AreEqual(2, trabajos.Count);
            Assert.AreEqual("TP1", trabajos[0].Titulo);
            Assert.AreEqual("TP2", trabajos[1].Titulo);
        }
    }
}
