using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a comisiones y a su calendario unificado (H1/H2).
    /// </summary>
    public interface IComisionRepository
    {
        /// <summary>Devuelve todas las comisiones ordenadas por código.</summary>
        List<Comision> ListarComisiones();

        /// <summary>Inserta una comisión nueva.</summary>
        /// <param name="comision">Comisión a crear (código y semanas).</param>
        void AgregarComision(Comision comision);

        /// <summary>Indica si ya existe una comisión con ese código.</summary>
        /// <param name="codigo">Código a verificar.</param>
        bool ExisteComision(string codigo);

        /// <summary>Devuelve clases y trabajos de la comisión unidos por fecha (H2, Escenario 3).</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<ElementoCalendario> VerCalendario(string codigoComision);
    }
}
