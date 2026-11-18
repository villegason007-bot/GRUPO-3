namespace Gestor_de_Estudiantes.Entidades;

/// <summary>
/// Registro de asistencia de un estudiante a una clase: qué condición
/// (pendiente, presente o ausente) quedó cargada para ese encuentro.
/// Es la primera de las dos señales que alimentan el indicador de
/// riesgo (H6).
/// </summary>
public class Asistencia
{
    /// <summary>Identificador único del registro.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Clave foránea a la <see cref="Clase"/> a la que corresponde.</summary>
    public Guid ClaseId { get; set; }

    /// <summary>Clave foránea al <see cref="Estudiante"/> al que corresponde.</summary>
    public Guid EstudianteId { get; set; }

    /// <summary>
    /// Relación de navegación con el estudiante; hoy las consultas del
    /// repositorio no la poblan (se llena solo si otro código la asigna).
    /// </summary>
    public Estudiante? Estudiante { get; set; }

    /// <summary>
    /// Relación de navegación con la clase; hoy las consultas del
    /// repositorio no la poblan (se llena solo si otro código la asigna).
    /// </summary>
    public Clase? Clase { get; set; }

    /// <summary>Condición del encuentro según el modelo de dominio (Pendiente | Presente | Ausente).</summary>
    public Condicion Condicion { get; set; }

    /// <summary>
    /// Vista booleana de <see cref="Condicion"/> para lectura: true solo
    /// si es Presente (Pendiente y Ausente devuelven false). Es de solo
    /// lectura: escribir directamente <see cref="Condicion"/> para no
    /// perder el tri-estado.
    /// </summary>
    public bool EstaPresente => Condicion == Condicion.Presente;
}
