namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato del cálculo puro del indicador de riesgo (H6): sin base de
    /// datos ni estado, solo conteos de entrada y puntaje de salida.
    /// </summary>
    public interface ICalculadoraDeRiesgo
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
        ResultadoRiesgo Calcular(
            int clasesHabiles, int faltas,
            int trabajosVencidos, int noEntregados,
            double umbral);
    }
}
