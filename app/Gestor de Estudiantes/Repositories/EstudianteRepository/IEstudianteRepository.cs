using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a la nómina de estudiantes (H1). Las consultas
    /// solo devuelven estudiantes activos; la baja es lógica.
    /// </summary>
    public interface IEstudianteRepository
    {
        /// <summary>Devuelve los estudiantes activos de la comisión, ordenados por legajo.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Estudiante> ListarEstudiantes(string codigoComision);

        /// <summary>Busca un estudiante activo por legajo.</summary>
        /// <param name="legajo">Legajo a buscar.</param>
        /// <returns>El estudiante, o null si no existe o está dado de baja.</returns>
        Estudiante? BuscarEstudiante(string legajo);

        /// <summary>Inserta un estudiante nuevo en la comisión (activo por defecto).</summary>
        /// <param name="estudiante">Estudiante a crear.</param>
        /// <param name="codigoComision">Comisión en la que se incorpora.</param>
        void AgregarEstudiante(Estudiante estudiante, string codigoComision);

        /// <summary>Da de baja lógica al estudiante por legajo (marca <c>activo = 0</c>).</summary>
        /// <param name="legajo">Legajo del estudiante a dar de baja.</param>
        /// <returns>true si estaba activo y se dio de baja.</returns>
        bool EliminarEstudiante(string legajo);
    }
}
