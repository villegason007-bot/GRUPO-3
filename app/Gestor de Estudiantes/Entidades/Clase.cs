namespace Gestor_de_Estudiantes.Entidades;

/// <summary>
/// Encuentro programado de una comisión: una clase del calendario con su
/// fecha y su estado hábil/no hábil.
/// </summary>
public class Clase
{
    /// <summary>Identificador único de la clase; lo usan las asistencias y los servicios para referenciarla.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Fecha del encuentro (solo fecha, sin hora).</summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Día hábil o no (feriado/suspensión): los no hábiles no reciben
    /// asistencia y se excluyen del cálculo de riesgo (H2 + H6).
    /// </summary>
    public EstadoClase Estado { get; set; } = EstadoClase.Habil;
}
