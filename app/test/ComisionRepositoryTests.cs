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

        [TestMethod]
        public void Given_ClasesYTrabajos_When_VerCalendario_Then_OrdenadoPorFechaMixto()
        {
            new ClaseRepository().AgregarClase(new Clase { Fecha = new DateTime(2026, 10, 19) }, "1K1");
            new ClaseRepository().AgregarClase(
                new Clase { Fecha = new DateTime(2026, 10, 5), Estado = EstadoClase.NoHabil }, "1K1");
            new TrabajoRepository().AgregarTrabajo(
                new Trabajo { Titulo = "TP1", FechaDeEntrega = new DateTime(2026, 10, 12) }, "1K1");

            var calendario = _repositorio.VerCalendario("1K1");

            Assert.AreEqual(3, calendario.Count);
            Assert.AreEqual(new DateTime(2026, 10, 5), calendario[0].Fecha);
            Assert.AreEqual("Clase", calendario[0].Tipo);
            Assert.AreEqual("NoHabil", calendario[0].Detalle);
            Assert.AreEqual(new DateTime(2026, 10, 12), calendario[1].Fecha);
            Assert.AreEqual("Trabajo", calendario[1].Tipo);
            Assert.AreEqual("TP1", calendario[1].Detalle);
            Assert.AreEqual(new DateTime(2026, 10, 19), calendario[2].Fecha);
            Assert.AreEqual("Clase", calendario[2].Tipo);
        }

        [TestMethod]
        public void Given_ComisionSinEventos_When_VerCalendario_Then_DevuelveVacio()
        {
            Assert.AreEqual(0, _repositorio.VerCalendario("1K1").Count);
        }
    }
}
