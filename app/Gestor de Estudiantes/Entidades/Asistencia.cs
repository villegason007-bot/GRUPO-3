namespace Gestor_de_Estudiantes.Entidades;

/// <summary>
/// Registro de asistencia de un estudiante a una clase: qué condición
/// (presente o ausente) quedó cargada para ese encuentro. Es la primera
/// de las dos señales que alimentan el indicador de riesgo (H6).
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

    /// <summary>Condición del encuentro según el modelo de dominio (Presente | Ausente).</summary>
    public Condicion Condicion { get; set; }

    /// <summary>
    /// Vista booleana de <see cref="Condicion"/> para las pantallas:
    /// leer devuelve true si es Presente; escribir traduce el bool a
    /// Presente o Ausente.
    /// </summary>
    public bool EstaPresente
    {
        get => Condicion == Condicion.Presente;
        set => Condicion = value ? Condicion.Presente : Condicion.Ausente;
    }
}
