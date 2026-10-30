using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a materias y a su vínculo N:M con comisiones
    /// a través de la tabla <c>materia_comision</c> (QA001).
    /// </summary>
    public interface IMateriaRepository
    {
        /// <summary>Devuelve todas las materias ordenadas por nombre.</summary>
        List<Materia> ListarMaterias();

        /// <summary>Inserta una materia nueva (el nombre debe ser único).</summary>
        /// <param name="materia">Materia a crear.</param>
        void AgregarMateria(Materia materia);

        /// <summary>
        /// Indica si ya existe una materia con ese nombre, sin distinguir
        /// mayúsculas de minúsculas.
        /// </summary>
        /// <param name="nombre">Nombre a verificar.</param>
        bool ExisteMateria(string nombre);

        /// <summary>
        /// Vincula una materia con una comisión (N:M). Si el vínculo ya
        /// existe no hace nada.
        /// </summary>
        /// <param name="nombreMateria">Nombre de la materia (debe existir).</param>
        /// <param name="codigoComision">Código de la comisión (debe existir).</param>
        void AsignarMateria(string nombreMateria, string codigoComision);

        /// <summary>
        /// Quita el vínculo entre una materia y una comisión.
        /// </summary>
        /// <param name="nombreMateria">Nombre de la materia.</param>
        /// <param name="codigoComision">Código de la comisión.</param>
        /// <returns>true si había un vínculo y se quitó.</returns>
        bool DesasignarMateria(string nombreMateria, string codigoComision);

        /// <summary>Devuelve las materias de la comisión ordenadas por nombre.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Materia> ListarMateriasDeComision(string codigoComision);

        /// <summary>Devuelve las comisiones donde se dicta la materia, ordenadas por código.</summary>
        /// <param name="nombreMateria">Nombre de la materia.</param>
        List<Comision> ListarComisionesDeMateria(string nombreMateria);
    }
}
