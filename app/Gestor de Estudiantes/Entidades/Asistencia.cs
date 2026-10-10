namespace Gestor_de_Estudiantes.Entidades;

public class Asistencia
{
    // Id de la fila en la base de datos
    public Guid Id { get; set; } = Guid.NewGuid();

    // Clave foránea a la clase
    public Guid ClaseId { get; set; }

    // Clave foránea al estudiante
    public Guid EstudianteId { get; set; }

    // Relación con el estudiante
    public Estudiante? Estudiante { get; set; }

    // Relación con la clase
    public Clase? Clase { get; set; }

    // El tipo de dato oficial requerido por el diagrama
    public Condicion Condicion { get; set; }

    // Propiedad puente que lee y modifica la condición
    public bool EstaPresente
    {
        get => Condicion == Condicion.Presente;
        set => Condicion = value ? Condicion.Presente : Condicion.Ausente;
    }
}
