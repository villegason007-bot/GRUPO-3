namespace Gestor_de_Estudiantes.Entidades;

/// <summary>
/// Condición registrada en una asistencia.
/// </summary>
public enum Condicion
{
    /// <summary>
    /// El estudiante todavía no fue marcado: estado por defecto del
    /// registro, antes de indicar asistencia o ausencia. No suma falta
    /// ni cuenta como información para el riesgo (H6).
    /// </summary>
    Pendiente,

    /// <summary>El estudiante asistió al encuentro.</summary>
    Presente,

    /// <summary>El estudiante faltó al encuentro; suma al indicador de riesgo (H6).</summary>
    Ausente
}
