using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Validaciones de alta de estudiantes (QA002): el legajo es
    /// alfanumérico ('abc!' o 'a b' no sirven) y único.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class EstudianteServiceTests : RepositorioTestBase
    {
        private readonly EstudianteService _servicio =
            new(new EstudianteRepository());

        [TestMethod]
        public void Given_LegajoAlfanumerico_When_AgregarEstudiante_Then_SeCrea()
        {
            var estudiante = _servicio.AgregarEstudiante("A1b2", "Ana", "1K1");

            Assert.AreEqual("A1b2", estudiante.Legajo);
            Assert.AreEqual(1, _servicio.ListarEstudiantes("1K1").Count);
        }

        [TestMethod]
        public void Given_LegajoConSimbolo_When_AgregarEstudiante_Then_LanzaArgumentException()
        {
            var excepcion = Assert.Throws<ArgumentException>(() =>
                _servicio.AgregarEstudiante("abc!", "Ana", "1K1"));

            StringAssert.Contains(excepcion.Message, "letras y números");
        }

        [TestMethod]
        public void Given_LegajoConEspacio_When_AgregarEstudiante_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.AgregarEstudiante("a b", "Ana", "1K1"));
        }

        [TestMethod]
        public void Given_LegajoRepetido_When_AgregarEstudiante_Then_LanzaInvalidOperationException()
        {
            _servicio.AgregarEstudiante("9001", "Ana", "1K1");

            Assert.Throws<InvalidOperationException>(() =>
                _servicio.AgregarEstudiante("9001", "Otra", "1K1"));
        }
    }
}
