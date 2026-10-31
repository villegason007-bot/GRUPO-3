using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Guardado de asistencia (H3/H4) con el tri-estado de
    /// <see cref="Condicion"/>: alta y corrección con Pendiente,
    /// Presente y Ausente, y compatibilidad del overload booleano.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class AsistenciaServiceTests : RepositorioTestBase
    {
        private readonly AsistenciaService _servicio =
            new(new AsistenciaRepository());

        [TestMethod]
        public void Given_EstudianteSinRegistro_When_GuardarPendiente_Then_CreaLaFilaSinMarcar()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();

            _servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Pendiente);

            var asistencias = _servicio.ListarPorClase(claseId);
            Assert.AreEqual(1, asistencias.Count);
            Assert.AreEqual(Condicion.Pendiente, asistencias[0].Condicion);
        }

        [TestMethod]
        public void Given_RegistroPendiente_When_GuardarPresente_Then_CorrigeSinDuplicar()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            _servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Pendiente);

            _servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Presente);

            var asistencias = _servicio.ListarPorClase(claseId);
            Assert.AreEqual(1, asistencias.Count);
            Assert.AreEqual(Condicion.Presente, asistencias[0].Condicion);
        }

        [TestMethod]
        public void Given_OverloadBooleano_When_Guardar_Then_TrueEsPresenteYFalseEsAusente()
        {
            var claseId = CrearClase();
            var ana = CrearEstudiante();
            var luis = CrearEstudiante("9002", "Luis");

            _servicio.GuardarAsistencia(claseId, ana, presente: true);
            _servicio.GuardarAsistencia(claseId, luis, presente: false);

            var asistencias = _servicio.ListarPorClase(claseId);
            Assert.AreEqual(2, asistencias.Count);
            Assert.AreEqual(Condicion.Presente,
                asistencias.Single(a => a.EstudianteId == ana).Condicion);
            Assert.AreEqual(Condicion.Ausente,
                asistencias.Single(a => a.EstudianteId == luis).Condicion);
        }

        [TestMethod]
        public void Given_ClaseNoHabil_When_Guardar_Then_LanzaInvalidOperationException()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            new ClaseRepository().MarcarEstado(claseId, EstadoClase.NoHabil);

            Assert.Throws<InvalidOperationException>(() =>
                _servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Pendiente));
        }
    }
}
