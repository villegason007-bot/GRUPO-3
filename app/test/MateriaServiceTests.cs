using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class MateriaServiceTests : RepositorioTestBase
    {
        private readonly MateriaService _servicio =
            new(new MateriaRepository(), new ComisionRepository());

        [TestMethod]
        public void Given_NombreValido_When_CrearMateria_Then_SeCrea()
        {
            var materia = _servicio.CrearMateria("  Matemática  ");

            Assert.AreEqual("Matemática", materia.Nombre);
            Assert.AreEqual(1, _servicio.ListarMaterias().Count);
        }

        [TestMethod]
        public void Given_NombreVacio_When_CrearMateria_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _servicio.CrearMateria("   "));
        }

        [TestMethod]
        public void Given_DuplicadaEnOtraCaja_When_CrearMateria_Then_LanzaInvalidOperationException()
        {
            _servicio.CrearMateria("Matematicas");

            var excepcion = Assert.Throws<InvalidOperationException>(() =>
                _servicio.CrearMateria("MATEMATICAS"));

            StringAssert.Contains(excepcion.Message, "MATEMATICAS");
        }

        [TestMethod]
        public void Given_MateriaYComision_When_Asignar_Then_ApareceEnAmbosListados()
        {
            _servicio.CrearMateria("Laboratorio");

            _servicio.AsignarMateria("Laboratorio", "1K1");

            Assert.AreEqual(1, _servicio.ListarMateriasDeComision("1K1").Count);
            Assert.AreEqual(1, _servicio.ListarComisionesDeMateria("Laboratorio").Count);
        }

        [TestMethod]
        public void Given_MateriaInexistente_When_Asignar_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.AsignarMateria("Fantasma", "1K1"));
        }

        [TestMethod]
        public void Given_ComisionInexistente_When_Asignar_Then_LanzaInvalidOperationException()
        {
            _servicio.CrearMateria("Laboratorio");

            Assert.Throws<InvalidOperationException>(() =>
                _servicio.AsignarMateria("Laboratorio", "9K9"));
        }

        [TestMethod]
        public void Given_Vinculo_When_Desasignar_Then_DevuelveTrueYDesaparece()
        {
            _servicio.CrearMateria("Historia");
            _servicio.AsignarMateria("Historia", "1K1");

            Assert.IsTrue(_servicio.DesasignarMateria("Historia", "1K1"));
            Assert.AreEqual(0, _servicio.ListarMateriasDeComision("1K1").Count);
        }
    }
}
