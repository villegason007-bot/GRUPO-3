using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de ficha del estudiante (H9): datos
    /// identificatorios y de contacto, con el historial de contactos
    /// registrados (H10 la muestra en la ficha).
    /// </summary>
    public interface IFichaService
    {
        /// <summary>
        /// Devuelve la ficha de un estudiante activo (H9).
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        /// <returns>La ficha con los datos y sus contactos.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        FichaEstudiante ObtenerFicha(string legajo);
    }

    /// <summary>
    /// Ficha del estudiante (H9): quién es, cómo contactarlo y qué
    /// contactos registró el tutor.
    /// </summary>
    public class FichaEstudiante
    {
        /// <summary>Identificador del estudiante.</summary>
        public Guid Id { get; set; }

        /// <summary>Legajo del estudiante.</summary>
        public string Legajo { get; set; } = string.Empty;

        /// <summary>Nombre del estudiante.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Apellido del estudiante.</summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Nombre completo (nombre + apellido).</summary>
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

        /// <summary>Teléfono de contacto (opcional).</summary>
        public string? Telefono { get; set; }

        /// <summary>Fecha de incorporación a la comisión (opcional).</summary>
        public string? FechaIncorporacion { get; set; }

        /// <summary>Código de la comisión donde está inscripto.</summary>
        public string CodigoComision { get; set; } = string.Empty;

        /// <summary>Contactos registrados por el tutor, del más reciente al más antiguo (H10).</summary>
        public List<Contacto> Contactos { get; set; } = new();

        /// <summary>Último contacto, o null si nunca fue contactado (H11).</summary>
        public Contacto? UltimoContacto => Contactos.Count > 0 ? Contactos[0] : null;
    }
}
