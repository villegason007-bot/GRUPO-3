using Gestor_de_Estudiantes.Entidades;

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

        /// <summary>
        /// Cuenta las ausencias seguidas vigentes (R1): desde la última
        /// clase con asistencia tomada hacia atrás, hasta la primera
        /// presencia.
        /// </summary>
        /// <param name="condicionesPorFecha">
        /// Condiciones del estudiante ordenadas de la clase más antigua a la
        /// más reciente. Las Pendiente no suman ni cortan la racha.
        /// </param>
        /// <returns>
        /// null si no hay ninguna asistencia tomada (sin datos); 0 si la
        /// última asistencia tomada es Presente; N si faltó las últimas N
        /// clases seguidas.
        /// </returns>
        int? CalcularAusenciasSeguidas(IReadOnlyList<Condicion> condicionesPorFecha);
    }
}