using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Historial de asistencia del estudiante (H12), con su propio
    /// contrato desde el split de SRP-2.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class HistorialAsistenciaServiceTests : RepositorioTestBase
    {
        private readonly HistorialAsistenciaService _servicio =
            new(new EstudianteRepository(), new AsistenciaRepository());

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
        public void Given_LegajoInexistente_When_HistorialAsistencia_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.HistorialAsistencia("0000"));
        }

        [TestMethod]
        public void Given_LegajoVacio_When_HistorialAsistencia_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.HistorialAsistencia("  "));
        }
    }
}
