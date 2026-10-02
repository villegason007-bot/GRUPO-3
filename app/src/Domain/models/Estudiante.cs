namespace Domain.Models;

public class Estudiante
{
    public string Legajo { get; }
    public string Nombre { get; }

    public Estudiante(string legajo, string nombre)
    {
        if (string.IsNullOrWhiteSpace(legajo))
            throw new ArgumentException("El legajo no puede estar vacío.", nameof(legajo));
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        Legajo = legajo.Trim();
        Nombre = nombre.Trim();
    }

    public override string ToString() => $"{Legajo} — {Nombre}";
}
