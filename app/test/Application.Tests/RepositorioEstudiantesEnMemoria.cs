using Application.Interfaces;
using Domain.Models;

namespace Application.Tests;

public class RepositorioEstudiantesEnMemoria : IRepositorioEstudiantes
{
    private readonly List<(string Comision, Estudiante Estudiante)> estudiantes = new();

    public void Agregar(string nombreComision, Estudiante estudiante)
        => estudiantes.Add((nombreComision, estudiante));

    public IReadOnlyList<Estudiante> Listar(string nombreComision)
        => estudiantes
            .Where(fila => fila.Comision == nombreComision)
            .Select(fila => fila.Estudiante)
            .ToList();

    public bool ExisteLegajo(string legajo)
        => estudiantes.Any(fila => fila.Estudiante.Legajo == legajo);
}
