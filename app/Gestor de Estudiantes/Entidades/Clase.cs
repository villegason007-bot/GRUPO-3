namespace Gestor_de_Estudiantes.Entidades;

public class Clase
{
    //  para que el servicio de asistencia pueda identificar la clase
    public Guid Id { get; set; } = Guid.NewGuid();

    // Fecha del encuentro (atributo del diagrama de dominio)
    public DateTime Fecha { get; set; }
}
