using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test
{
    /// <summary>
    /// Criterios de aceptación de R2 · Incorporación tardía.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class R2IncorporacionTardiaAcceptanceTests : RepositorioTestBase
    {
        private readonly RiesgoService _servicio =
            new(new RiesgoRepository(), new ParametrosRiesgoRepository(), new EstudianteRepository(), new CalculadoraDeRiesgo(), new ContactoRepository());

        private readonly AsistenciaRepository _asistencias = new();

        /// <summary>Crea un estudiante con (o sin) fecha de incorporación y devuelve su id.</summary>
        private static Guid CrearEstudianteConFecha(string legajo, string nombre, string? fechaIncorporacion)
        {
            var repositorio = new EstudianteRepository();
            repositorio.AgregarEstudiante(new Estudiante
            {
                Legajo = legajo,
                Nombre = nombre,
                Apellido = "Test",
                FechaIncorporacion = fechaIncorporacion,
            }, "1K1");
            return repositorio.ListarEstudiantes("1K1").First(e => e.Legajo == legajo).Id;
        }

        /// <summary>Crea una clase por fecha y registra la condición del estudiante en cada una.</summary>
        private void CargarAsistencias(Guid estudianteId, params (string Fecha, Condicion Condicion)[] clases)
        {
            foreach (var (fecha, condicion) in clases)
            {
                var claseId = CrearClase(DateTime.Parse(fecha));
                _asistencias.RegistrarAsistencia(new Asistencia
                {
                    ClaseId = claseId,
                    EstudianteId = estudianteId,
                    Condicion = condicion,
                });
            }
        }

        private static void CrearTrabajoVencido(string titulo, DateTime fecha)
        {
            new TrabajoRepository().AgregarTrabajo(
                new Trabajo { Titulo = titulo, FechaDeEntrega = fecha }, "1K1");
        }

        private ResultadoRiesgo Riesgo(string legajo) =>
            _servicio.CalcularRiesgos("1K1").Single(r => r.Legajo == legajo);

        [TestMethod]
        public void Given_IncorporadoEl15DeSeptiembreCon4ClasesYUnaFalta_When_CalcularRiesgos_Then_1De4Clases25Porciento()
        {
            // 8 clases con asistencia tomada; Díaz entra el 15/09 y desde ahí hay 4 clases.
            var diaz = CrearEstudianteConFecha("2001", "Diaz", "2026-09-15");
            CargarAsistencias(diaz,
                ("2026-08-18", Condicion.Ausente),
                ("2026-08-25", Condicion.Ausente),
                ("2026-09-01", Condicion.Ausente),
                ("2026-09-08", Condicion.Ausente),
                ("2026-09-15", Condicion.Presente),
                ("2026-09-22", Condicion.Ausente),
                ("2026-09-29", Condicion.Presente),
                ("2026-10-06", Condicion.Presente));

            var resultado = Riesgo("2001");

            Assert.AreEqual(4, resultado.ClasesHabiles);   // y no 8
            Assert.AreEqual(1, resultado.Faltas);          // y no 5
            Assert.AreEqual(25, resultado.Puntaje);
        }

        [TestMethod]
        public void Given_TP1VencidoAntesYTP2Despues_When_CalcularRiesgos_Then_1NoEntregadoDe1()
        {
            // TP1 vence el 10/09 y TP2 el 30/09; Díaz entra el 15/09 y no entrega ninguno.
            CrearEstudianteConFecha("2001", "Diaz", "2026-09-15");
            CrearTrabajoVencido("TP1", new DateTime(2026, 9, 10));
            CrearTrabajoVencido("TP2", new DateTime(2026, 9, 30));

            var resultado = Riesgo("2001");

            Assert.AreEqual(1, resultado.TrabajosVencidos); // y no 2
            Assert.AreEqual(1, resultado.NoEntregados);     // y no 2
        }

        [TestMethod]
        public void Given_IncorporadoHoySinClasesNiVencimientosDesdeEntonces_When_CalcularRiesgos_Then_SinDatos()
        {
            var hoy = DateTime.Today.ToString("yyyy-MM-dd");
            var vega = CrearEstudianteConFecha("2002", "Vega", hoy);
            CargarAsistencias(vega,
                (DateTime.Today.AddDays(-14).ToString("yyyy-MM-dd"), Condicion.Ausente),
                (DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"), Condicion.Ausente));
            CrearTrabajoVencido("TP1", DateTime.Today.AddDays(-10));

            var resultado = Riesgo("2002");

            Assert.IsTrue(resultado.SinInformacion);   // no 0 % ni riesgo bajo
            Assert.IsNull(resultado.Puntaje);
            Assert.IsFalse(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_SinFechaDeIncorporacion_When_CalcularRiesgos_Then_CuentaLas8ClasesYLos2Trabajos()
        {
            var perez = CrearEstudianteConFecha("2003", "Perez", null);
            CargarAsistencias(perez,
                ("2026-08-18", Condicion.Presente),
                ("2026-08-25", Condicion.Ausente),
                ("2026-09-01", Condicion.Presente),
                ("2026-09-08", Condicion.Presente),
                ("2026-09-15", Condicion.Presente),
                ("2026-09-22", Condicion.Ausente),
                ("2026-09-29", Condicion.Presente),
                ("2026-10-06", Condicion.Presente));
            CrearTrabajoVencido("TP1", new DateTime(2026, 9, 10));
            CrearTrabajoVencido("TP2", new DateTime(2026, 9, 30));

            var resultado = Riesgo("2003");

            Assert.AreEqual(8, resultado.ClasesHabiles);
            Assert.AreEqual(2, resultado.Faltas);
            Assert.AreEqual(2, resultado.TrabajosVencidos);
            Assert.AreEqual(2, resultado.NoEntregados);
        }

        [TestMethod]
        public void Given_FaltasAnterioresALaIncorporacion_When_CalcularRiesgos_Then_NoCuentanParaLaRacha()
        {
            // Faltó en las 2 clases previas a su llegada y estuvo presente en la última.
            var diaz = CrearEstudianteConFecha("2001", "Diaz", "2026-09-15");
            CargarAsistencias(diaz,
                ("2026-09-01", Condicion.Ausente),
                ("2026-09-08", Condicion.Ausente),
                ("2026-09-15", Condicion.Presente));

            var resultado = Riesgo("2001");

            Assert.AreEqual(0, resultado.AusenciasSeguidas);
        }
    }
}