namespace Gestor_de_Estudiantes.Entidades;

internal class Asistencia
{
    // Id para identificar este registro de asistencia si fuera necesario
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relación con el estudiante 
    public Estudiante Estudiante { get; set; }

    // Relación con la clase 
    public Clase Clase { get; set; }

    // El estado de la asistencia: True = Presente, False = Ausente
    public bool EstaPresente { get; set; }

    // Opcional para el Sprint 4 o ampliaciones (Ausencias justificadas, etc.)
    // public string Observaciones { get; set; } 
}
