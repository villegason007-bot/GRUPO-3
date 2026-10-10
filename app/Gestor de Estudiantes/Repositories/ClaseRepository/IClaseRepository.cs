using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IClaseRepository
    {
        List<Clase> ListarClases(string codigoComision);
        void AgregarClase(Clase clase, string codigoComision);
        bool MarcarEstado(Guid claseId, EstadoClase estado);
    }
}
