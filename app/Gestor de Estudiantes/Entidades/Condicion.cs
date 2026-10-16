namespace Gestor_de_Estudiantes.Entidades;

/// <summary>
/// Condición registrada en una asistencia.
/// </summary>
public enum Condicion
{
    /// <summary>El estudiante asistió al encuentro.</summary>
    Presente,

    /// <summary>El estudiante faltó al encuentro; suma al indicador de riesgo (H6).</summary>
    Ausente
}
