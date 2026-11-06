using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Historial de entregas del estudiante (H13), con su propio
    /// contrato desde el split de SRP-2.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class HistorialEntregasServiceTests : RepositorioTestBase
    {
        private readonly HistorialEntregasService _servicio =
            new(new EstudianteRepository(), new EntregaRepository());

        [TestMethod]
        public void Given_TrabajoConEntrega_When_HistorialEntregas_Then_EstadoEntregado()
        {
            var estudianteId = CrearEstudiante();
            var trabajoId = CrearTrabajo(new DateTime(2026, 10, 1));
            new EntregaRepository().RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = true,
                Fecha = new DateTime(2026, 9, 28),
            });

            var historial = _servicio.HistorialEntregas("9001");

            Assert.AreEqual(1, historial.Count);
            Assert.AreEqual("TP1", historial[0].Titulo);
            Assert.IsTrue(historial[0].Entregado);
            Assert.IsFalse(historial[0].SinEntregar);
            Assert.AreEqual(new DateTime(2026, 9, 28), historial[0].Fecha);
        }

        [TestMethod]
        public void Given_TrabajoSinRegistro_When_HistorialEntregas_Then_EstadoSinEntregar()
        {
            CrearEstudiante();
            CrearTrabajo(new DateTime(2026, 10, 1));

            var historial = _servicio.HistorialEntregas("9001");

            Assert.AreEqual(1, historial.Count);
            Assert.IsTrue(historial[0].SinEntregar);
            Assert.IsNull(historial[0].Fecha);
        }

        [TestMethod]
        public void Given_ComisionSinTrabajos_When_HistorialEntregas_Then_Vacio()
        {
            CrearEstudiante();

            Assert.AreEqual(0, _servicio.HistorialEntregas("9001").Count);
        }

        [TestMethod]
        public void Given_LegajoInexistente_When_HistorialEntregas_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.HistorialEntregas("0000"));
        }

        [TestMethod]
        public void Given_LegajoVacio_When_HistorialEntregas_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.HistorialEntregas("  "));
        }
    }
}
