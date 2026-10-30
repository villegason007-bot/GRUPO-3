using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Calendario unificado (H2, Escenario 3) extraído de comisiones:
    /// clases y trabajos de la comisión unidos por fecha.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class CalendarioServiceTests : RepositorioTestBase
    {
        private readonly CalendarioService _servicio =
            new(new CalendarioRepository());

        [TestMethod]
        public void Given_ClaseYTrabajo_When_VerCalendario_Then_LosUnePorFecha()
        {
            CrearTrabajo(new DateTime(2026, 10, 10));
            CrearClase(new DateTime(2026, 10, 5));

            var calendario = _servicio.VerCalendario("1K1");

            Assert.AreEqual(2, calendario.Count);
            Assert.AreEqual(new DateTime(2026, 10, 5), calendario[0].Fecha);
            Assert.AreEqual("Clase", calendario[0].Tipo);
            Assert.AreEqual(new DateTime(2026, 10, 10), calendario[1].Fecha);
            Assert.AreEqual("Trabajo", calendario[1].Tipo);
            Assert.AreEqual("TP1", calendario[1].Detalle);
        }

        [TestMethod]
        public void Given_ComisionSinEventos_When_VerCalendario_Then_Vacio()
        {
            Assert.AreEqual(0, _servicio.VerCalendario("1K1").Count);
        }
    }
}
