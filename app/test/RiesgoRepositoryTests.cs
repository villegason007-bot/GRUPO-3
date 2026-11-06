using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class RiesgoRepositoryTests : RepositorioTestBase
    {
        private readonly RiesgoRepository _repositorio = new();

        [TestMethod]
        public void Given_AsistenciaEnClaseHabil_When_ContarAsistencias_Then_CuentaFaltas()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            var asistencias = new AsistenciaRepository();
            asistencias.RegistrarAsistencia(new Gestor_de_Estudiantes.Entidades.Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Gestor_de_Estudiantes.Entidades.Condicion.Ausente,
            });

            var (clasesHabiles, faltas, conRegistro) =
                _repositorio.ContarAsistencias(estudianteId, "1K1");

            Assert.AreEqual(1, clasesHabiles);
            Assert.AreEqual(1, faltas);
            Assert.IsTrue(conRegistro);
        }

        [TestMethod]
        public void Given_AsistenciaRegistradaYLuegoClaseNoHabil_When_ContarAsistencias_Then_SeExcluyeDelRiesgo()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            new AsistenciaRepository().RegistrarAsistencia(new Gestor_de_Estudiantes.Entidades.Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Gestor_de_Estudiantes.Entidades.Condicion.Ausente,
            });

            new ClaseRepository().MarcarEstado(claseId, Gestor_de_Estudiantes.Entidades.EstadoClase.NoHabil);

            var (clasesHabiles, faltas, _) =
                _repositorio.ContarAsistencias(estudianteId, "1K1");

            Assert.AreEqual(0, clasesHabiles);
            Assert.AreEqual(0, faltas);
        }

        [TestMethod]
        public void Given_SinAsistenciaRegistrada_When_ContarAsistencias_Then_DevuelveSinRegistro()
        {
            CrearClase();
            var estudianteId = CrearEstudiante();

            var (clasesHabiles, faltas, conRegistro) =
                _repositorio.ContarAsistencias(estudianteId, "1K1");

            Assert.AreEqual(1, clasesHabiles); // las clases hábiles de la comisión existen
            Assert.AreEqual(0, faltas);
            Assert.IsFalse(conRegistro); // pero el estudiante no tiene registro → sin info
        }

        [TestMethod]
        public void Given_TrabajoVencidoSinEntrega_When_ContarTrabajos_Then_VenceYSinEntregar()
        {
            var estudianteId = CrearEstudiante();
            new TrabajoRepository().AgregarTrabajo(
                new Gestor_de_Estudiantes.Entidades.Trabajo
                {
                    Titulo = "TP viejo",
                    FechaDeEntrega = new DateTime(2026, 10, 1),
                },
                "1K1");

            var (vencidos, entregados) = _repositorio.ContarTrabajos(estudianteId, "1K1");

            Assert.AreEqual(1, vencidos);
            Assert.AreEqual(0, entregados);
        }

        [TestMethod]
        public void Given_TrabajoFuturo_When_ContarTrabajos_Then_NoSeCuentaComoVencido()
        {
            CrearTrabajo(DateTime.Today.AddDays(30)); // fecha futura

            var (vencidos, entregados) = _repositorio.ContarTrabajos(Guid.NewGuid(), "1K1");

            Assert.AreEqual(0, vencidos);
            Assert.AreEqual(0, entregados);
        }

        [TestMethod]
        public void Given_SoloRegistroPendiente_When_ContarAsistencias_Then_NoEsFaltaNiInformacion()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            new AsistenciaRepository().RegistrarAsistencia(new Gestor_de_Estudiantes.Entidades.Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Gestor_de_Estudiantes.Entidades.Condicion.Pendiente,
            });

            var (clasesHabiles, faltas, conRegistro) =
                _repositorio.ContarAsistencias(estudianteId, "1K1");

            Assert.AreEqual(1, clasesHabiles);
            Assert.AreEqual(0, faltas, "Un registro sin marcar no es falta.");
            Assert.IsFalse(conRegistro, "Sin Presente/Ausente el estudiante sigue sin información.");
        }

        [TestMethod]
        public void Given_PendienteYAusente_When_ContarAsistencias_Then_SoloCuentaElAusente()
        {
            var estudianteId = CrearEstudiante();
            var repositorioAsistencias = new AsistenciaRepository();
            var clasePendiente = CrearClase(new DateTime(2026, 10, 5));
            var claseAusente = CrearClase(new DateTime(2026, 10, 12));
            repositorioAsistencias.RegistrarAsistencia(new Gestor_de_Estudiantes.Entidades.Asistencia
            {
                ClaseId = clasePendiente,
                EstudianteId = estudianteId,
                Condicion = Gestor_de_Estudiantes.Entidades.Condicion.Pendiente,
            });
            repositorioAsistencias.RegistrarAsistencia(new Gestor_de_Estudiantes.Entidades.Asistencia
            {
                ClaseId = claseAusente,
                EstudianteId = estudianteId,
                Condicion = Gestor_de_Estudiantes.Entidades.Condicion.Ausente,
            });

            var (clasesHabiles, faltas, conRegistro) =
                _repositorio.ContarAsistencias(estudianteId, "1K1");

            Assert.AreEqual(2, clasesHabiles);
            Assert.AreEqual(1, faltas, "Solo la ausente suma falta.");
            Assert.IsTrue(conRegistro);
        }
    }
}
