using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    [DoNotParallelize]
    public sealed class RiesgoServiceTests : RepositorioTestBase
    {
        private readonly RiesgoService _servicio =
            new(new RiesgoRepository(), new EstudianteRepository(), new CalculadoraDeRiesgo(), new ContactoRepository());

        private readonly ParametrosRiesgoService _parametros =
            new(new RiesgoRepository());

        [TestMethod]
        public void Given_EstudianteSinDatos_When_CalcularRiesgos_Then_SinInformacion()
        {
            CrearEstudiante();

            var resultados = _servicio.CalcularRiesgos("1K1");

            Assert.AreEqual(1, resultados.Count);
            Assert.IsTrue(resultados[0].SinInformacion);
            Assert.IsNull(resultados[0].Puntaje);
        }

        [TestMethod]
        public void Given_EstudianteSinAsistenciaPeroConClases_When_CalcularRiesgos_Then_SinInformacion()
        {
            CrearClase();
            CrearEstudiante();

            var resultados = _servicio.CalcularRiesgos("1K1");

            Assert.IsTrue(resultados.Single().SinInformacion);
        }

        [TestMethod]
        public void Given_EstudianteConFaltasYTrabajos_When_CalcularRiesgos_Then_PuntajeEsperado()
        {
            var estudianteId = CrearEstudiante();
            var repositorioAsistencias = new AsistenciaRepository();
            var claseId = CrearClase();
            var segundaClaseId = CrearClase(new DateTime(2026, 10, 12));
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Ausente,
            });
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = segundaClaseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });
            var trabajoId = CrearTrabajo(new DateTime(2026, 10, 1));

            // faltas 1/2 = 50% · no entregados 1/1 = 100% → promedio 75
            var resultados = _servicio.CalcularRiesgos("1K1");

            var resultado = resultados.Single();
            Assert.IsFalse(resultado.SinInformacion);
            Assert.AreEqual(75, resultado.Puntaje);
            Assert.IsTrue(resultado.EnRiesgo); // umbral 50
            Assert.AreEqual("9001", resultado.Legajo);
        }

        [TestMethod]
        public void Given_UmbralModificado_When_CalcularRiesgos_Then_UsaElNuevoValor()
        {
            var estudianteId = CrearEstudiante();
            var repositorioAsistencias = new AsistenciaRepository();
            var claseId = CrearClase();
            var segundaClaseId = CrearClase(new DateTime(2026, 10, 12));
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Ausente,
            });
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = segundaClaseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Presente,
            });

            var antes = _servicio.CalcularRiesgos("1K1").Single();
            Assert.IsTrue(antes.EnRiesgo); // 50% de faltas >= umbral 50

            _parametros.GuardarUmbral(90);

            var despues = _servicio.CalcularRiesgos("1K1").Single();
            Assert.AreEqual(50, despues.Puntaje);
            Assert.IsFalse(despues.EnRiesgo); // 50 < 90 → el nuevo umbral manda
        }

        [TestMethod]
        public void Given_EstudianteSinContactos_When_CalcularRiesgos_Then_SinContactar()
        {
            CrearEstudiante();

            var resultado = _servicio.CalcularRiesgos("1K1").Single();

            Assert.IsTrue(resultado.SinContactar);
            Assert.IsNull(resultado.FechaUltimoContacto);
            Assert.AreEqual("Sin contactar", resultado.TextoUltimoContacto);
        }

        [TestMethod]
        public void Given_EstudianteContactado_When_CalcularRiesgos_Then_TieneLaFechaDelUltimoContacto()
        {
            var estudianteId = CrearEstudiante();
            new ContactoRepository().RegistrarContacto(new Contacto
            {
                EstudianteId = estudianteId,
                Fecha = DateTime.Today.AddDays(-1),
                Medio = MedioContacto.Llamada,
                Resultado = ResultadoContacto.Contesto,
            });

            var resultado = _servicio.CalcularRiesgos("1K1").Single();

            Assert.IsFalse(resultado.SinContactar);
            Assert.AreEqual(DateTime.Today.AddDays(-1), resultado.FechaUltimoContacto);
        }

        [TestMethod]
        public void Given_EstudianteSinDatos_When_CalcularRiesgoDe_Then_SinInformacion()
        {
            CrearEstudiante();

            var resultado = _servicio.CalcularRiesgoDe("9001");

            Assert.IsTrue(resultado.SinInformacion);
            Assert.AreEqual("9001", resultado.Legajo);
        }

        [TestMethod]
        public void Given_EstudianteConFaltas_When_CalcularRiesgoDe_Then_DevuelvePuntaje()
        {
            var estudianteId = CrearEstudiante();
            var repositorioAsistencias = new AsistenciaRepository();
            var claseId = CrearClase();
            repositorioAsistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = Condicion.Ausente,
            });

            var resultado = _servicio.CalcularRiesgoDe("9001");

            Assert.AreEqual(100, resultado.Puntaje); // 1/1 = 100% de faltas
            Assert.IsTrue(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_LegajoInexistente_When_CalcularRiesgoDe_Then_LanzaInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _servicio.CalcularRiesgoDe("0000"));
        }

        [TestMethod]
        public void Given_LegajoVacio_When_CalcularRiesgoDe_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _servicio.CalcularRiesgoDe("  "));
        }
    }
}
