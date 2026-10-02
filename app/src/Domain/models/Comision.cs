namespace Domain.Models;

public class Comision
{
    private readonly List<Estudiante> estudiantes = new();

    public string Nombre { get; }
    public IReadOnlyList<Estudiante> Estudiantes => estudiantes;

    public Comision(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la comisión no puede estar vacío.", nameof(nombre));

        Nombre = nombre.Trim();
    }

    public void Inscribir(Estudiante estudiante)
    {
        ArgumentNullException.ThrowIfNull(estudiante);

        if (estudiantes.Any(e => e.Legajo == estudiante.Legajo))
            throw new InvalidOperationException($"El estudiante {estudiante.Legajo} ya está inscripto en {Nombre}.");

        estudiantes.Add(estudiante);
    }
}
