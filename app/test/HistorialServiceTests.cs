using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class HistorialServiceTests : RepositorioTestBase
    {
        private readonly HistorialService _servicio =
            new(new EstudianteRepository(), new AsistenciaRepository(), new EntregaRepository());

        [TestMethod]
        public void Given_AsistenciasRegistradas_When_HistorialAsistencia_Then_FechasYCondiciones()
        {
            var estudianteId = CrearEstudiante();
            var repositorioAsistencias = new AsistenciaRepository();
            var claseId = CrearClase(new DateTime(2026, 10, 5));
            var segundaClaseId = CrearClase(new DateTime(2026, 10, 12));
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = segundaClaseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Ausente,
            });

            var historial = _servicio.HistorialAsistencia("9001");

            Assert.AreEqual(2, historial.Count);
            Assert.AreEqual(new DateTime(2026, 10, 5), historial[0].Fecha);
            Assert.IsTrue(historial[0].EstaPresente);
            Assert.AreEqual(new DateTime(2026, 10, 12), historial[1].Fecha);
            Assert.IsFalse(historial[1].EstaPresente);
        }

        [TestMethod]
        public void Given_EstudianteSinRegistros_When_HistorialAsistencia_Then_Vacio()
        {
            CrearEstudiante();
            CrearClase();

            Assert.AreEqual(0, _servicio.HistorialAsistencia("9001").Count);
        }

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
        public void Given_LegajoInexistente_When_ConsultarHistoriales_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.HistorialAsistencia("0000"));
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.HistorialEntregas("0000"));
        }
    }
}
