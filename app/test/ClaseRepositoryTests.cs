using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ClaseRepositoryTests : RepositorioTestBase
    {
        private readonly ClaseRepository _repositorio = new();

        [TestMethod]
        public void Given_ComisionSinClases_When_ListarClases_Then_DevuelveVacia()
        {
            var clases = _repositorio.ListarClases("1K1");

            Assert.AreEqual(0, clases.Count);
        }

        [TestMethod]
        public void Given_DosClases_When_ListarClases_Then_DevuelveOrdenadasPorFecha()
        {
            _repositorio.AgregarClase(new Clase { Fecha = new DateTime(2026, 10, 12) }, "1K1");
            _repositorio.AgregarClase(new Clase { Fecha = new DateTime(2026, 10, 5) }, "1K1");

            var clases = _repositorio.ListarClases("1K1");

            Assert.AreEqual(2, clases.Count);
            Assert.AreEqual(new DateTime(2026, 10, 5), clases[0].Fecha);
            Assert.AreEqual(new DateTime(2026, 10, 12), clases[1].Fecha);
        }

        [TestMethod]
        public void Given_NuevaClase_When_AgregarClase_Then_ApareceEnElListado()
        {
            var fecha = new DateTime(2026, 10, 5);

            _repositorio.AgregarClase(new Clase { Fecha = fecha }, "1K1");

            var clases = _repositorio.ListarClases("1K1");
            Assert.AreEqual(1, clases.Count);
            Assert.AreEqual(fecha, clases[0].Fecha);
            Assert.AreNotEqual(Guid.Empty, clases[0].Id);
        }

        [TestMethod]
        public void Given_NuevaClase_When_AgregarClase_Then_EstadoPorDefectoHabil()
        {
            _repositorio.AgregarClase(new Clase { Fecha = new DateTime(2026, 10, 5) }, "1K1");

            var clases = _repositorio.ListarClases("1K1");

            Assert.AreEqual(EstadoClase.Habil, clases[0].Estado);
        }

        [TestMethod]
        public void Given_ClaseHabil_When_MarcarEstado_Then_QuedaNoHabil()
        {
            var claseId = CrearClase();

            var marcada = _repositorio.MarcarEstado(claseId, EstadoClase.NoHabil);

            Assert.IsTrue(marcada);
            var clases = _repositorio.ListarClases("1K1");
            Assert.AreEqual(EstadoClase.NoHabil, clases.Single(clase => clase.Id == claseId).Estado);
        }

        [TestMethod]
        public void Given_ClaseInexistente_When_MarcarEstado_Then_DevuelveFalse()
        {
            Assert.IsFalse(_repositorio.MarcarEstado(Guid.NewGuid(), EstadoClase.NoHabil));
        }
    }
}
