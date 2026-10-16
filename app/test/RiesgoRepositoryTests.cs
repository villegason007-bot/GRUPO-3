using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class RiesgoRepositoryTests : RepositorioTestBase
    {
        private readonly RiesgoRepository _repositorio = new();

        [TestMethod]
        public void Given_BDRecienCreada_When_ObtenerUmbral_Then_Es50()
        {
            Assert.AreEqual(50, _repositorio.ObtenerUmbral());
        }

        [TestMethod]
        public void Given_UmbralModificado_When_ObtenerUmbral_Then_DevuelveElNuevo()
        {
            _repositorio.GuardarUmbral(75);

            Assert.AreEqual(75, _repositorio.ObtenerUmbral());
        }

        [TestMethod]
        public void Given_UmbralInvalido_When_GuardarUmbral_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _repositorio.GuardarUmbral(-1));
            Assert.Throws<ArgumentException>(() => _repositorio.GuardarUmbral(101));
        }

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
            CrearTrabajo(); // fecha 2026-10-20, futura

            var (vencidos, entregados) = _repositorio.ContarTrabajos(Guid.NewGuid(), "1K1");

            Assert.AreEqual(0, vencidos);
            Assert.AreEqual(0, entregados);
        }
    }
}
