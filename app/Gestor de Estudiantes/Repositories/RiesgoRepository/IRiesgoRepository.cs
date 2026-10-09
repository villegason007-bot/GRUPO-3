using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public interface IRiesgoRepository
    {
        double ObtenerUmbral();
        void GuardarUmbral(double umbral);
        (int ClasesHabiles, int Faltas, bool TieneRegistro) ContarAsistencias(
            Guid estudianteId, string codigoComision);
        (int TrabajosVencidos, int Entregados) ContarTrabajos(Guid estudianteId, string codigoComision);
    }
}
