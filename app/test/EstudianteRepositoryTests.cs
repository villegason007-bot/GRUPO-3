using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class EstudianteRepositoryTests : RepositorioTestBase
    {
        private readonly EstudianteRepository _repositorio = new();

        [TestMethod]
        public void Given_EstudianteConAsistenciasYEntregas_When_EliminarEstudiante_Then_DevuelveTrueYConservaLaFila()
        {
            var legajo = "9001";
            var claseId = CrearClase();
            var estudianteId = CrearEstudiante();
            var trabajoId = CrearTrabajo();
            var asistencias = new AsistenciaRepository();
            var entregas = new EntregaRepository();
            asistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });
            entregas.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = true,
                Fecha = new DateTime(2026, 10, 19),
            });

            var eliminado = _repositorio.EliminarEstudiante(legajo);

            Assert.IsTrue(eliminado);
            var (total, activo) = ContarPorLegajo(legajo);
            Assert.AreEqual(1, total, "La baja es logica: la fila fisica debe conservarse.");
            Assert.AreEqual(0, activo);
            Assert.AreEqual(1, asistencias.ListarPorClase(claseId).Count, "El historial de asistencias no se toca.");
            Assert.AreEqual(1, entregas.ListarPorTrabajo(trabajoId).Count, "El historial de entregas no se toca.");
        }

        [TestMethod]
        public void Given_EstudianteDadoDeBaja_When_ListarOBuscar_Then_NoAparece()
        {
            var legajo = "9001";
            CrearEstudiante();
            _repositorio.EliminarEstudiante(legajo);

            Assert.AreEqual(0, _repositorio.ListarEstudiantes("1K1").Count);
            Assert.IsNull(_repositorio.BuscarEstudiante(legajo));
        }

        [TestMethod]
        public void Given_EstudianteDadoDeBaja_When_EliminarOtraVez_Then_DevuelveFalse()
        {
            var legajo = "9001";
            CrearEstudiante();
            Assert.IsTrue(_repositorio.EliminarEstudiante(legajo));

            Assert.IsFalse(_repositorio.EliminarEstudiante(legajo));
        }

        [TestMethod]
        public void Given_EstudianteActivo_When_EliminarEstudiante_Then_NoAfectaALosDemas()
        {
            CrearEstudiante();
            var otroLegajo = "9002";
            _repositorio.AgregarEstudiante(new Estudiante { Legajo = otroLegajo, Nombre = "Luis" }, "1K1");

            Assert.IsTrue(_repositorio.EliminarEstudiante("9001"));

            var activos = _repositorio.ListarEstudiantes("1K1");
            Assert.AreEqual(1, activos.Count);
            Assert.AreEqual(otroLegajo, activos[0].Legajo);
        }

        private static (int Total, int Activo) ContarPorLegajo(string legajo)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT COUNT(*), COALESCE(SUM(activo), 0)
                FROM estudiantes
                WHERE legajo = @legajo;
                """;
            comando.Parameters.AddWithValue("@legajo", legajo);
            using var lector = comando.ExecuteReader();
            lector.Read();
            return (lector.GetInt32(0), lector.GetInt32(1));
        }
    }
}
