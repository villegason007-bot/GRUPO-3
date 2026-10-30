using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class FichaServiceTests : RepositorioTestBase
    {
        private readonly FichaService _servicio =
            new(new EstudianteRepository(), new ContactoRepository());

        [TestMethod]
        public void Given_Estudiante_When_ObtenerFicha_Then_DatosIdentificatorios()
        {
            CrearEstudiante();

            var ficha = _servicio.ObtenerFicha("9001");

            Assert.AreEqual("9001", ficha.Legajo);
            Assert.AreEqual("Ana", ficha.Nombre);
            Assert.AreEqual("García", ficha.Apellido);
            Assert.AreEqual("Ana García", ficha.NombreCompleto);
            Assert.AreEqual("1K1", ficha.CodigoComision);
        }

        [TestMethod]
        public void Given_ContactosRegistrados_When_ObtenerFicha_Then_LosIncluye()
        {
            CrearEstudiante();
            new ContactoService(
                new ContactoRepository(), new EstudianteRepository())
                .RegistrarContacto("9001", MedioContacto.Llamada, ResultadoContacto.Contesto);

            var ficha = _servicio.ObtenerFicha("9001");

            Assert.AreEqual(1, ficha.Contactos.Count);
            Assert.IsNotNull(ficha.UltimoContacto);
            Assert.AreEqual(DateTime.Today, ficha.UltimoContacto.Fecha);
        }

        [TestMethod]
        public void Given_EstudianteSinContactos_When_ObtenerFicha_Then_UltimoContactoNull()
        {
            CrearEstudiante();

            var ficha = _servicio.ObtenerFicha("9001");

            Assert.AreEqual(0, ficha.Contactos.Count);
            Assert.IsNull(ficha.UltimoContacto);
        }

        [TestMethod]
        public void Given_LegajoVacio_When_ObtenerFicha_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _servicio.ObtenerFicha("  "));
        }

        [TestMethod]
        public void Given_LegajoInexistente_When_ObtenerFicha_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _servicio.ObtenerFicha("0000"));
        }
    }
}
