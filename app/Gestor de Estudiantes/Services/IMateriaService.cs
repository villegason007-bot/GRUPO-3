using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de materias (QA001): alta validada y
    /// consulta/actualización del vínculo N:M con comisiones.
    /// </summary>
    public interface IMateriaService
    {
        /// <summary>Devuelve todas las materias ordenadas por nombre.</summary>
        List<Materia> ListarMaterias();

        /// <summary>
        /// Crea una materia validando nombre obligatorio y único sin
        /// distinguir mayúsculas de minúsculas.
        /// </summary>
        /// <param name="nombre">Nombre de la materia.</param>
        /// <returns>La materia creada.</returns>
        /// <exception cref="ArgumentException">Si el nombre está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una materia con ese nombre.</exception>
        Materia CrearMateria(string nombre);

        /// <summary>Asigna una materia a una comisión (N:M).</summary>
        /// <param name="nombreMateria">Materia (debe existir).</param>
        /// <param name="codigoComision">Comisión (debe existir).</param>
        /// <exception cref="InvalidOperationException">Si no existen la materia o la comisión.</exception>
        void AsignarMateria(string nombreMateria, string codigoComision);

        /// <summary>
        /// Desasigna una materia de una comisión.
        /// </summary>
        /// <returns>true si había un vínculo y se quitó.</returns>
        bool DesasignarMateria(string nombreMateria, string codigoComision);

        /// <summary>Devuelve las materias de la comisión ordenadas por nombre.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Materia> ListarMateriasDeComision(string codigoComision);

        /// <summary>Devuelve las comisiones donde se dicta la materia.</summary>
        /// <param name="nombreMateria">Nombre de la materia.</param>
        List<Comision> ListarComisionesDeMateria(string nombreMateria);
    }
}
