using Domain.Models;

namespace Application.Interfaces;

public interface IRepositorioEstudiantes
{
    void Agregar(string nombreComision, Estudiante estudiante);
    IReadOnlyList<Estudiante> Listar(string nombreComision);
    bool ExisteLegajo(string legajo);
}
