using Gestor_de_Estudiantes.Services;

namespace Test
{
    [TestClass]
    public sealed class CalculadoraDeRiesgoTests
    {
        private readonly CalculadoraDeRiesgo _calculadora = new();

        [TestMethod]
        public void Given_SinClasesNiTrabajos_When_Calcular_Then_SinInformacion()
        {
            var resultado = _calculadora.Calcular(0, 0, 0, 0, umbral: 50);

            Assert.IsTrue(resultado.SinInformacion);
            Assert.IsNull(resultado.Puntaje);
            Assert.IsFalse(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_SoloTrabajosSinClases_When_Calcular_Then_UsaSoloElComponenteDeEntregas()
        {
            var resultado = _calculadora.Calcular(0, 0, 4, 4, umbral: 50);

            Assert.AreEqual(100, resultado.Puntaje);
            Assert.IsTrue(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_AsistenciaSinFaltas_When_Calcular_Then_PuntajeCero()
        {
            var resultado = _calculadora.Calcular(10, 0, 0, 0, umbral: 50);

            Assert.AreEqual(0, resultado.Puntaje);
            Assert.IsFalse(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_TodasLasClasesAusentes_When_Calcular_Then_Puntaje100EnRiesgo()
        {
            var resultado = _calculadora.Calcular(10, 10, 0, 0, umbral: 50);

            Assert.AreEqual(100, resultado.Puntaje);
            Assert.IsTrue(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_FaltasYNoEntregados_When_Calcular_Then_PromedioDeAmbosComponentes()
        {
            // 5/10 faltas = 50% · 2/2 no entregados = 100% → promedio 75
            var resultado = _calculadora.Calcular(10, 5, 2, 2, umbral: 50);

            Assert.AreEqual(75, resultado.Puntaje);
        }

        [TestMethod]
        public void Given_PuntajeBajoElUmbral_When_Calcular_Then_NoEnRiesgo()
        {
            var resultado = _calculadora.Calcular(10, 4, 0, 0, umbral: 50);
            // El estado es híbrido: el límite de faltas debe quedar por encima
            // para que decida solo el puntaje (como cuando la UI guarda los topes).
            resultado.MaxFaltasPermitidas = 5;

            Assert.AreEqual(40, resultado.Puntaje);
            Assert.IsFalse(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_MismoPuntajePeroUmbralMasBajo_When_Calcular_Then_SiEnRiesgo()
        {
            var resultado = _calculadora.Calcular(10, 4, 0, 0, umbral: 30);

            Assert.AreEqual(40, resultado.Puntaje);
            Assert.IsTrue(resultado.EnRiesgo);
        }

        [TestMethod]
        public void Given_FaltasMayoresQueClases_When_Calcular_Then_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => _calculadora.Calcular(5, 6, 0, 0, umbral: 50));
        }
    }
}
