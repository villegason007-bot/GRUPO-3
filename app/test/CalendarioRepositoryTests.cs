using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    /// <summary>
    /// Calendario unificado (H2, Escenario 3) extraído de
    /// ComisionRepository a CalendarioRepository (SRP).
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class CalendarioRepositoryTests : RepositorioTestBase
    {
        private readonly CalendarioRepository _repositorio = new();

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
            Assert.AreEqual("CLASE", calendario[0].Tipo);
            Assert.AreEqual("Clase", calendario[0].Titulo);
            Assert.AreEqual("Estado: NoHabil", calendario[0].Detalle);
            Assert.AreEqual(new DateTime(2026, 10, 12), calendario[1].Fecha);
            Assert.AreEqual("TRABAJO", calendario[1].Tipo);
            Assert.AreEqual("TP1", calendario[1].Titulo);
            Assert.AreEqual("Fecha de entrega", calendario[1].Detalle);
            Assert.AreEqual(new DateTime(2026, 10, 19), calendario[2].Fecha);
            Assert.AreEqual("CLASE", calendario[2].Tipo);
        }

        [TestMethod]
        public void Given_ComisionSinEventos_When_VerCalendario_Then_DevuelveVacio()
        {
            Assert.AreEqual(0, _repositorio.VerCalendario("1K1").Count);
        }

        [TestMethod]
        public void Given_ClaseYTrabajoEnLaMismaFecha_When_VerCalendario_Then_LaClaseVaPrimero()
        {
            var mismaFecha = new DateTime(2026, 10, 19);
            new ClaseRepository().AgregarClase(new Clase { Fecha = mismaFecha }, "1K1");
            new TrabajoRepository().AgregarTrabajo(
                new Trabajo { Titulo = "TP1", FechaDeEntrega = mismaFecha }, "1K1");

            var calendario = _repositorio.VerCalendario("1K1");

            Assert.AreEqual(2, calendario.Count);
            Assert.AreEqual("CLASE", calendario[0].Tipo);
            Assert.AreEqual("TRABAJO", calendario[1].Tipo);
        }
    }
}
