using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class MateriaRepositoryTests : RepositorioTestBase
    {
        private readonly MateriaRepository _repositorio = new();

        [TestMethod]
        public void Given_NuevaMateria_When_Agregar_Then_ApareceEnElListado()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Matemática" });

            var materias = _repositorio.ListarMaterias();

            Assert.AreEqual(1, materias.Count);
            Assert.AreEqual("Matemática", materias[0].Nombre);
        }

        [TestMethod]
        public void Given_MateriaEnCajaMinuscula_When_ExisteEnMayusculas_Then_DevuelveTrue()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Matematicas" });

            Assert.IsTrue(_repositorio.ExisteMateria("MATEMATICAS"));
            Assert.IsTrue(_repositorio.ExisteMateria("matematicas"));
        }

        [TestMethod]
        public void Given_MateriaDuplicada_When_Agregar_Then_LanzaPorLaUnicaNOCASE()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Matematicas" });

            Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() =>
                _repositorio.AgregarMateria(new Materia { Nombre = "MATEMATICAS" }));
        }

        [TestMethod]
        public void Given_MateriaAsignada_When_ListarMateriasDeComision_Then_LaDevuelve()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Laboratorio" });
            _repositorio.AsignarMateria("Laboratorio", "1K1");

            var materias = _repositorio.ListarMateriasDeComision("1K1");

            Assert.AreEqual(1, materias.Count);
            Assert.AreEqual("Laboratorio", materias[0].Nombre);
        }

        [TestMethod]
        public void Given_MismaMateriaEnDosComisiones_When_ListarPorLado_Then_LasDevuelveAmbas()
        {
            new ComisionRepository().AgregarComision(new Comision { Codigo = "2K2" });
            _repositorio.AgregarMateria(new Materia { Nombre = "Física" });
            _repositorio.AsignarMateria("Física", "1K1");
            _repositorio.AsignarMateria("Física", "2K2");

            Assert.AreEqual(
                2,
                _repositorio.ListarComisionesDeMateria("Física").Count);
            Assert.AreEqual(
                1,
                _repositorio.ListarMateriasDeComision("1K1").Count);
            Assert.AreEqual(
                1,
                _repositorio.ListarMateriasDeComision("2K2").Count);
        }

        [TestMethod]
        public void Given_VinculoRepetido_When_Asignar_Then_NoSeDuplica()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Química" });
            _repositorio.AsignarMateria("Química", "1K1");
            _repositorio.AsignarMateria("Química", "1K1");

            Assert.AreEqual(1, _repositorio.ListarMateriasDeComision("1K1").Count);
        }

        [TestMethod]
        public void Given_VinculoExistente_When_Desasignar_Then_SeQuita()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Historia" });
            _repositorio.AsignarMateria("Historia", "1K1");

            var seQuito = _repositorio.DesasignarMateria("Historia", "1K1");

            Assert.IsTrue(seQuito);
            Assert.AreEqual(0, _repositorio.ListarMateriasDeComision("1K1").Count);
        }

        [TestMethod]
        public void Given_SinVinculo_When_Desasignar_Then_DevuelveFalse()
        {
            _repositorio.AgregarMateria(new Materia { Nombre = "Geografía" });

            Assert.IsFalse(_repositorio.DesasignarMateria("Geografía", "1K1"));
        }
    }
}
