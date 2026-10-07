namespace Gestor_de_Estudiantes.Entidades;

internal class Asistencia
{
    // Id opcional para identificar la fila
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relación con el estudiante
    public Estudiante Estudiante { get; set; }

    // Relación con la clase
    public Clase Clase { get; set; }

    // El tipo de dato oficial requerido por el diagrama 
    public Condicion Condicion { get; set; }

    // Propiedad puente que lee y modifica la condición
    public bool EstaPresente 
    { 
        get => Condicion == Condicion.Presente;
        set => Condicion = value ? Condicion.Presente : Condicion.Ausente;
    }
}
