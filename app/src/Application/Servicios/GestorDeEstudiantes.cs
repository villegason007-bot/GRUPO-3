using Application.Interfaces;
using Domain.Models;

namespace Application.Servicios;

public class GestorDeEstudiantes
{
    private readonly IRepositorioEstudiantes repositorio;

    public GestorDeEstudiantes(IRepositorioEstudiantes repositorio)
    {
        this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public Estudiante AgregarEstudiante(string nombreComision, string legajo, string nombre)
    {
        var estudiante = new Estudiante(legajo, nombre);

        if (repositorio.ExisteLegajo(estudiante.Legajo))
            throw new InvalidOperationException($"Ya existe un estudiante con legajo {estudiante.Legajo}.");

        repositorio.Agregar(nombreComision, estudiante);
        return estudiante;
    }

    public IReadOnlyList<Estudiante> ListarEstudiantes(string nombreComision)
        => repositorio.Listar(nombreComision);
}
