namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Estado de una clase en el calendario: un día no hábil (feriado,
    /// suspensión u otro problema) no recibe asistencia y no entra al
    /// cálculo de riesgo.
    /// </summary>
    public enum EstadoClase
    {
        /// <summary>Día normal: recibe asistencia y cuenta en el riesgo.</summary>
        Habil,

        /// <summary>Feriado o suspensión: no recibe asistencia y se excluye del cálculo de riesgo (H2 + H6).</summary>
        NoHabil,
    }
}
