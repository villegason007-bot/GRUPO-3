using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Cálculo puro del indicador de riesgo (H6), sin base de datos ni
    /// estado: riesgo = promedio entre % de faltas en clases hábiles y
    /// % de trabajos vencidos no entregados (0-100).
    /// Sin datos de ningún tipo → sin información (nunca división por cero).
    /// </summary>
    public class CalculadoraDeRiesgo : ICalculadoraDeRiesgo
    {
        /// <summary>
        /// Calcula el puntaje de riesgo de un estudiante a partir de sus
        /// conteos. Valida que las cantidades sean coherentes y promedia
        /// solo los componentes con datos disponibles.
        /// </summary>
        /// <param name="clasesHabiles">Clases hábiles de la comisión (denominador de faltas).</param>
        /// <param name="faltas">Ausencias del estudiante en clases hábiles.</param>
        /// <param name="trabajosVencidos">Trabajos de la comisión ya vencidos a hoy (denominador).</param>
        /// <param name="noEntregados">De los vencidos, cuántos no entregó el estudiante.</param>
        /// <param name="umbral">Umbral porcentual (0-100) a partir del cual el puntaje se considera en riesgo.</param>
        /// <returns>Resultado con puntaje 0-100, o sin puntaje si no hay ningún componente con datos.</returns>
        /// <exception cref="ArgumentException">Si alguna cantidad es negativa o supera su denominador.</exception>
        public ResultadoRiesgo Calcular(
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

        /// <summary>
        /// Cuenta las ausencias seguidas vigentes (R1): recorre las
        /// condiciones desde la más reciente hacia atrás y se detiene en
        /// la primera presencia. Las Pendiente se ignoran: no suman ni
        /// cortan la racha (CA3).
        /// </summary>
        /// <param name="condicionesPorFecha">Condiciones de la clase más antigua a la más reciente.</param>
        /// <returns>null si no hay asistencia tomada (CA4); 0 si la última es Presente (CA2); N ausencias seguidas (CA1).</returns>
        public int? CalcularAusenciasSeguidas(IReadOnlyList<Condicion> condicionesPorFecha)
        {
            var tomadas = condicionesPorFecha
                .Where(c => c != Condicion.Pendiente)
                .ToList();

            if (tomadas.Count == 0)
                return null;

            var racha = 0;
            for (var i = tomadas.Count - 1; i >= 0; i--)
            {
                if (tomadas[i] != Condicion.Ausente)
                    break;

                racha++;
            }

            return racha;
        }
    }
}