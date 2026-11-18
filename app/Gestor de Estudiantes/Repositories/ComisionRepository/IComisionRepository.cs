using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a comisiones (H1).
    /// </summary>
    public interface IComisionRepository
    {
        /// <summary>Devuelve todas las comisiones ordenadas por código.</summary>
        List<Comision> ListarComisiones();

        /// <summary>Inserta una comisión nueva.</summary>
        /// <param name="comision">Comisión a crear (código y semanas).</param>
        void AgregarComision(Comision comision);

        /// <summary>Indica si ya existe una comisión con ese código, sin distinguir mayúsculas.</summary>
        /// <param name="codigo">Código a verificar.</param>
        bool ExisteComision(string codigo);
    }
}
