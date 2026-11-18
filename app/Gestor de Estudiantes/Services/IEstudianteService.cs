using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de la nómina de estudiantes (H1):
    /// listado, búsqueda por legajo, alta, baja lógica y corrección
    /// de la fecha de incorporación (R2).
    /// </summary>
    public interface IEstudianteService
    {
        /// <summary>Devuelve los estudiantes activos de una comisión, ordenados por legajo.</summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        List<Estudiante> ListarEstudiantes(string codigoComision);

        /// <summary>
        /// Busca un estudiante activo por su legajo.
        /// </summary>
        /// <param name="legajo">Legajo a buscar (obligatorio).</param>
        /// <returns>El estudiante, o null si no existe o está dado de baja.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        Estudiante? BuscarEstudiante(string legajo);

        /// <summary>
        /// Da de alta un estudiante en la comisión (H1), validando campos
        /// obligatorios y legajo único.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (obligatorio, único).</param>
        /// <param name="nombre">Nombre del estudiante (obligatorio).</param>
        /// <param name="codigoComision">Comisión en la que se incorpora.</param>
        /// <param name="telefono">Teléfono de contacto (opcional; dígitos, espacios y + - ( )).</param>
        /// <param name="fechaIncorporacion">
        /// Fecha desde la que cursa (R2, opcional, formato aaaa-MM-dd). Vacía
        /// o nula: el estudiante cursa desde el inicio.
        /// </param>
        /// <returns>El estudiante creado.</returns>
        /// <exception cref="ArgumentException">Si legajo o nombre están vacíos, el teléfono tiene símbolos no permitidos o la fecha es inválida o posterior a hoy.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un estudiante con ese legajo.</exception>
        Estudiante AgregarEstudiante(
            string legajo, string nombre, string codigoComision,
            string? telefono = null, string? fechaIncorporacion = null);

        /// <summary>
        /// Da de baja lógica al estudiante: la fila queda en la base y se
        /// oculta de los listados (H1).
        /// </summary>
        /// <param name="legajo">Legajo del estudiante a dar de baja (obligatorio).</param>
        /// <returns>true si se dio de baja; false si no estaba activo.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        bool EliminarEstudiante(string legajo);

        /// <summary>
        /// Corrige la fecha de incorporación de un estudiante ya cargado (R2).
        /// Fecha vacía = quitarla (el riesgo vuelve a calcularse sobre todas las clases).
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (obligatorio).</param>
        /// <param name="fechaIncorporacion">Nueva fecha aaaa-MM-dd, o vacía para quitarla.</param>
        /// <returns>true si se actualizó; false si el estudiante no existe o está dado de baja.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío, o la fecha es inválida o posterior a hoy.</exception>
        bool CambiarFechaIncorporacion(string legajo, string? fechaIncorporacion);
    }
}