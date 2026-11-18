using Gestor_de_Estudiantes.Config;
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

        [TestMethod]
        public void Given_ClaseNoHabil_When_RegistrarAsistencia_Then_LanzaInvalidOperationException()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            new ClaseRepository().MarcarEstado(claseId, EstadoClase.NoHabil);

            var excepcion = Assert.Throws<InvalidOperationException>(
                () => _repositorio.RegistrarAsistencia(new Asistencia
                {
                    ClaseId = claseId,
                    EstudianteId = estudianteId,
                    Condicion = Condicion.Presente,
                }));

            Assert.IsTrue(excepcion.Message.Contains("no es hábil"));
            Assert.AreEqual(0, _repositorio.ListarPorClase(claseId).Count);
        }

        [TestMethod]
        public void Given_CondicionPendiente_When_Registrar_Then_PersisteSinLanzar()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();

            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Pendiente,
            });

            var asistencias = _repositorio.ListarPorClase(claseId);
            Assert.AreEqual(1, asistencias.Count);
            Assert.AreEqual(Condicion.Pendiente, asistencias[0].Condicion);
            Assert.IsFalse(asistencias[0].EstaPresente);
        }

        [TestMethod]
        public void Given_BaseConCheckViejo_When_Inicializar_Then_MigraElCheckYPreservaLosDatos()
        {
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            var idRegistro = Guid.NewGuid();

            // Se simula una base creada antes de Condicion.Pendiente:
            // tabla asistencias con el CHECK viejo de dos valores y una
            // fila ya cargada.
            using (var conexion = BaseDeDatos.Abrir())
            using (var comando = conexion.CreateCommand())
            {
                comando.CommandText = """
                    PRAGMA foreign_keys = OFF;
                    DROP TABLE asistencias;
                    CREATE TABLE asistencias (
                        id TEXT PRIMARY KEY,
                        clase_id TEXT NOT NULL REFERENCES clases(id),
                        estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                        condicion TEXT NOT NULL CHECK (condicion IN ('Presente', 'Ausente')),
                        UNIQUE (clase_id, estudiante_id)
                    );
                    INSERT INTO asistencias (id, clase_id, estudiante_id, condicion)
                    VALUES (@id, @claseId, @estudianteId, 'Presente');
                    """;
                comando.Parameters.AddWithValue("@id", idRegistro.ToString("D"));
                comando.Parameters.AddWithValue("@claseId", claseId.ToString("D"));
                comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
                comando.ExecuteNonQuery();
            }

            BaseDeDatos.Inicializar();

            // Los datos se conservan y el CHECK ahora acepta Pendiente.
            var preservada = _repositorio.ListarPorClase(claseId);
            Assert.AreEqual(1, preservada.Count);
            Assert.AreEqual(Condicion.Presente, preservada[0].Condicion);
            Assert.AreEqual(idRegistro, preservada[0].Id);

            var segundoEstudianteId = CrearEstudiante("9002", "Luis");
            _repositorio.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = segundoEstudianteId,
                Condicion = Condicion.Pendiente,
            });

            Assert.AreEqual(2, _repositorio.ListarPorClase(claseId).Count);

            // Idempotente: una segunda inicialización no vuelve a migrar.
            BaseDeDatos.Inicializar();
            Assert.AreEqual(2, _repositorio.ListarPorClase(claseId).Count);
        }
    }
}
