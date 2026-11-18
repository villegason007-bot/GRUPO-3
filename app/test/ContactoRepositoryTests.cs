using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ContactoRepositoryTests : RepositorioTestBase
    {
        private readonly ContactoRepository _repositorio = new();

        [TestMethod]
        public void Given_Contacto_When_Registrar_Then_ApareceEnElListado()
        {
            var estudianteId = CrearEstudiante();

            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = estudianteId,
                Fecha = DateTime.Today,
                Medio = MedioContacto.Llamada,
                Resultado = ResultadoContacto.Contesto,
            });

            var contactos = _repositorio.ListarContactos(estudianteId);

            Assert.AreEqual(1, contactos.Count);
            Assert.AreEqual(MedioContacto.Llamada, contactos[0].Medio);
            Assert.AreEqual(ResultadoContacto.Contesto, contactos[0].Resultado);
        }

        [TestMethod]
        public void Given_DosContactos_When_UltimoContacto_Then_DevuelveElMasReciente()
        {
            var estudianteId = CrearEstudiante();
            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = estudianteId,
                Fecha = DateTime.Today.AddDays(-7),
                Medio = MedioContacto.Llamada,
                Resultado = ResultadoContacto.SinRespuesta,
            });
            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = estudianteId,
                Fecha = DateTime.Today,
                Medio = MedioContacto.Mensaje,
                Resultado = ResultadoContacto.Contesto,
            });

            var ultimo = _repositorio.UltimoContacto(estudianteId);

            Assert.IsNotNull(ultimo);
            Assert.AreEqual(DateTime.Today, ultimo.Fecha);
            Assert.AreEqual(MedioContacto.Mensaje, ultimo.Medio);
        }

        [TestMethod]
        public void Given_EstudianteSinContactos_When_UltimoContacto_Then_Null()
        {
            Assert.IsNull(_repositorio.UltimoContacto(CrearEstudiante()));
        }

        [TestMethod]
        public void Given_DosEstudiantesContactados_When_UltimoContactoPorEstudiante_Then_TraeElDeCadaUno()
        {
            var ana = CrearEstudiante();
            var otro = CrearEstudiante("9002", "Bruno");
            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = ana,
                Fecha = DateTime.Today,
                Medio = MedioContacto.Mensaje,
                Resultado = ResultadoContacto.QuedoEnVeremos,
            });
            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = otro,
                Fecha = DateTime.Today.AddDays(-2),
                Medio = MedioContacto.Llamada,
                Resultado = ResultadoContacto.Contesto,
            });

            var ultimos = _repositorio.UltimoContactoPorEstudiante("1K1");

            Assert.AreEqual(2, ultimos.Count);
            Assert.AreEqual(ResultadoContacto.QuedoEnVeremos, ultimos[ana].Resultado);
            Assert.AreEqual(DateTime.Today.AddDays(-2), ultimos[otro].Fecha);
        }

        [TestMethod]
        public void Given_EstudianteDeOtraComision_When_UltimoContactoPorEstudiante_Then_NoAparece()
        {
            new ComisionRepository().AgregarComision(new Comision { Codigo = "2K2" });
            new EstudianteRepository().AgregarEstudiante(
                new Estudiante { Legajo = "7001", Nombre = "Carla" }, "2K2");
            var carla = new EstudianteRepository().BuscarEstudiante("7001")!;
            _repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = carla.Id,
                Fecha = DateTime.Today,
                Medio = MedioContacto.Mensaje,
                Resultado = ResultadoContacto.Contesto,
            });

            Assert.AreEqual(0, _repositorio.UltimoContactoPorEstudiante("1K1").Count);
        }

        [TestMethod]
        public void Given_MedioInvalido_When_Registrar_Then_LanzaPorElCheck()
        {
            var estudianteId = CrearEstudiante();

            Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() =>
                _repositorio.RegistrarContacto(new Contacto
                {
                    EstudianteId = estudianteId,
                    Fecha = DateTime.Today,
                    Medio = (MedioContacto)99,
                    Resultado = ResultadoContacto.Contesto,
                }));
        }
    }
}
