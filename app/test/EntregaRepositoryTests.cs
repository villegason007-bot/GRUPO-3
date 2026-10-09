using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Microsoft.Data.Sqlite;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class EntregaRepositoryTests : RepositorioTestBase
    {
        private readonly EntregaRepository _repositorio = new();

        [TestMethod]
        public void Given_NuevaEntrega_When_Registrar_Then_ApareceEnElListado()
        {
            var trabajoId = CrearTrabajo();
            var estudianteId = CrearEstudiante();
            var fecha = new DateTime(2026, 10, 19);

            _repositorio.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = true,
                Fecha = fecha,
            });

            var entregas = _repositorio.ListarPorTrabajo(trabajoId);
            Assert.AreEqual(1, entregas.Count);
            Assert.IsTrue(entregas[0].Entregado);
            Assert.AreEqual(fecha, entregas[0].Fecha);
        }

        [TestMethod]
        public void Given_EntregaSinFecha_When_Registrar_Then_QuedaSinFecha()
        {
            var trabajoId = CrearTrabajo();
            var estudianteId = CrearEstudiante();

            _repositorio.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = false,
                Fecha = null,
            });

            var entregas = _repositorio.ListarPorTrabajo(trabajoId);
            Assert.AreEqual(1, entregas.Count);
            Assert.IsFalse(entregas[0].Entregado);
            Assert.IsNull(entregas[0].Fecha);
        }

        [TestMethod]
        public void Given_EntregaRegistrada_When_ActualizarEntrega_Then_CambiaSinDuplicar()
        {
            var trabajoId = CrearTrabajo();
            var estudianteId = CrearEstudiante();
            _repositorio.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = false,
                Fecha = null,
            });

            var corregida = _repositorio.ActualizarEntrega(trabajoId, estudianteId, true, new DateTime(2026, 10, 21));

            var entregas = _repositorio.ListarPorTrabajo(trabajoId);
            Assert.IsTrue(corregida);
            Assert.AreEqual(1, entregas.Count);
            Assert.IsTrue(entregas[0].Entregado);
            Assert.AreEqual(new DateTime(2026, 10, 21), entregas[0].Fecha);
        }

        [TestMethod]
        public void Given_EntregaInexistente_When_ActualizarEntrega_Then_DevuelveFalse()
        {
            var trabajoId = CrearTrabajo();
            var estudianteId = CrearEstudiante();

            var corregida = _repositorio.ActualizarEntrega(trabajoId, estudianteId, true, null);

            Assert.IsFalse(corregida);
        }

        [TestMethod]
        public void Given_EntregaDuplicada_When_Registrar_Then_LanzaRestriccionUnica()
        {
            var trabajoId = CrearTrabajo();
            var estudianteId = CrearEstudiante();
            _repositorio.RegistrarEntrega(new Entrega
            {
                TrabajoId = trabajoId,
                EstudianteId = estudianteId,
                Entregado = true,
                Fecha = new DateTime(2026, 10, 19),
            });

            try
            {
                _repositorio.RegistrarEntrega(new Entrega
                {
                    TrabajoId = trabajoId,
                    EstudianteId = estudianteId,
                    Entregado = false,
                    Fecha = null,
                });
                Assert.Fail("Se esperaba una restricción UNIQUE.");
            }
            catch (SqliteException excepcion)
            {
                Assert.AreEqual(19, excepcion.SqliteErrorCode);
            }
        }
    }
}
