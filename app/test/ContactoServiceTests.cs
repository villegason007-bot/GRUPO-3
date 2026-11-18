using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ContactoServiceTests : RepositorioTestBase
    {
        private readonly ContactoService _servicio =
            new(new ContactoRepository(), new EstudianteRepository());

        [TestMethod]
        public void Given_LegajoValido_When_RegistrarContacto_Then_GuardaConLaFechaDelDia()
        {
            CrearEstudiante();

            var contacto = _servicio.RegistrarContacto(
                "9001", MedioContacto.Mensaje, ResultadoContacto.SinRespuesta);

            Assert.AreEqual(DateTime.Today, contacto.Fecha);
            Assert.AreEqual(MedioContacto.Mensaje, contacto.Medio);
            Assert.AreEqual(ResultadoContacto.SinRespuesta, contacto.Resultado);
        }

        [TestMethod]
        public void Given_LegajoVacio_When_RegistrarContacto_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.RegistrarContacto("  ", MedioContacto.Llamada, ResultadoContacto.Contesto));
        }

        [TestMethod]
        public void Given_LegajoInexistente_When_RegistrarContacto_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.RegistrarContacto("0000", MedioContacto.Llamada, ResultadoContacto.Contesto));
        }

        [TestMethod]
        public void Given_DosContactos_When_Listar_Then_OrdenaDelMasReciente()
        {
            CrearEstudiante();
            var repositorio = new ContactoRepository();
            var estudianteId = new EstudianteRepository().BuscarEstudiante("9001")!.Id;
            repositorio.RegistrarContacto(new Contacto
            {
                EstudianteId = estudianteId,
                Fecha = DateTime.Today.AddDays(-3),
                Medio = MedioContacto.Llamada,
                Resultado = ResultadoContacto.Contesto,
            });
            _servicio.RegistrarContacto("9001", MedioContacto.Mensaje, ResultadoContacto.Contesto);

            var contactos = _servicio.ListarContactos("9001");

            Assert.AreEqual(2, contactos.Count);
            Assert.AreEqual(DateTime.Today, contactos[0].Fecha);
            Assert.AreEqual(DateTime.Today.AddDays(-3), contactos[1].Fecha);
        }

        [TestMethod]
        public void Given_EstudianteSinContactos_When_UltimoContacto_Then_Null()
        {
            CrearEstudiante();

            Assert.IsNull(_servicio.UltimoContacto("9001"));
        }

        [TestMethod]
        public void Given_Comision_When_UltimoContactoPorEstudiante_Then_DevuelveElMapa()
        {
            CrearEstudiante();
            _servicio.RegistrarContacto("9001", MedioContacto.Llamada, ResultadoContacto.Contesto);

            var ultimos = _servicio.UltimoContactoPorEstudiante("1K1");

            Assert.AreEqual(1, ultimos.Count);
        }
    }
}
