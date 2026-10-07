using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Microsoft.Data.Sqlite;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class AsistenciaRepositoryTests : RepositorioTestBase
    {
        private readonly AsistenciaRepository _repositorio = new();

        [TestMethod]
        public void Given_DosAsistencias_When_Registrar_Then_ListarPorClaseLasDevuelve()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            var segundoEstudianteId = CrearEstudiante("9002", "Luis");

            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = segundoEstudianteId,
                Condicion = Condicion.Ausente,
            });

            var asistencias = _repositorio.ListarPorClase(claseId);

            Assert.AreEqual(2, asistencias.Count);
            Assert.AreNotEqual(Guid.Empty, asistencias[0].Id);
        }

        [TestMethod]
        public void Given_AsistenciaRegistrada_When_ActualizarCondicion_Then_CambiaSinDuplicar()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });

            var corregida = _repositorio.ActualizarCondicion(claseId, estudianteId, Condicion.Ausente);

            var asistencias = _repositorio.ListarPorClase(claseId);
            Assert.IsTrue(corregida);
            Assert.AreEqual(1, asistencias.Count);
            Assert.AreEqual(Condicion.Ausente, asistencias[0].Condicion);
        }

        [TestMethod]
        public void Given_CondicionInexistente_When_ActualizarCondicion_Then_DevuelveFalse()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();

            var corregida = _repositorio.ActualizarCondicion(claseId, estudianteId, Condicion.Presente);

            Assert.IsFalse(corregida);
        }

        [TestMethod]
        public void Given_AsistenciaDuplicada_When_Registrar_Then_LanzaRestriccionUnica()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });

            try
            {
                _repositorio.RegistrarAsistencia(new Asistencia
                {
                    ClaseId = claseId,
                    EstudianteId = estudianteId,
                    Condicion = Condicion.Ausente,
                });
                Assert.Fail("Se esperaba una restricción UNIQUE.");
            }
            catch (SqliteException excepcion)
            {
                Assert.AreEqual(19, excepcion.SqliteErrorCode);
            }
        }

        [TestMethod]
        public void Given_AsistenciasDeDosEstudiantes_When_ListarPorEstudiante_Then_DevuelveSoloLasDelEstudiante()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            var segundoEstudianteId = CrearEstudiante("9002", "Luis");
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = segundoEstudianteId,
                Condicion = Condicion.Ausente,
            });

            var asistencias = _repositorio.ListarPorEstudiante(estudianteId);

            Assert.AreEqual(1, asistencias.Count);
            Assert.AreEqual(estudianteId, asistencias[0].EstudianteId);
        }
    }
}
