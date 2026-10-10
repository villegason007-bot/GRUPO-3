namespace Gestor_de_Estudiantes.Services
{
    // Cálculo puro del indicador de riesgo (H6):
    //   riesgo = promedio entre % de faltas en clases hábiles y
    //            % de trabajos vencidos no entregados (0-100).
    // Sin datos de ningún tipo → sin información (nunca división por cero).
    public static class CalculadoraDeRiesgo
    {
        public static ResultadoRiesgo Calcular(
            int clasesHabiles, int faltas,
            int trabajosVencidos, int noEntregados,
            double umbral)
        {
            if (clasesHabiles < 0 || faltas < 0 || trabajosVencidos < 0 || noEntregados < 0)
                throw new ArgumentException("Las cantidades de riesgo no pueden ser negativas.");
            if (faltas > clasesHabiles)
                throw new ArgumentException("Las faltas no pueden superar las clases hábiles.");
            if (noEntregados > trabajosVencidos)
                throw new ArgumentException("Los no entregados no pueden superar los trabajos vencidos.");

            var resultado = new ResultadoRiesgo
            {
                ClasesHabiles = clasesHabiles,
                Faltas = faltas,
                TrabajosVencidos = trabajosVencidos,
                NoEntregados = noEntregados,
                Umbral = umbral,
            };

            double suma = 0;
            var componentes = 0;

            if (clasesHabiles > 0)
            {
                suma += (double)faltas / clasesHabiles * 100;
                componentes++;
            }

            if (trabajosVencidos > 0)
            {
                suma += (double)noEntregados / trabajosVencidos * 100;
                componentes++;
            }

            if (componentes == 0)
                return resultado; // sin datos: Escenario 2 de H6

            resultado.Puntaje = (int)Math.Round(suma / componentes, MidpointRounding.AwayFromZero);
            return resultado;
        }
    }
}
