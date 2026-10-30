using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a los contactos del tutor con sus estudiantes
    /// (H10/H11).
    /// </summary>
    public interface IContactoRepository
    {
        /// <summary>Inserta un contacto nuevo.</summary>
        /// <param name="contacto">Contacto a crear (estudiante, fecha, medio y resultado).</param>
        void RegistrarContacto(Contacto contacto);

        /// <summary>Devuelve los contactos de un estudiante, del más reciente al más antiguo.</summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        List<Contacto> ListarContactos(Guid estudianteId);

        /// <summary>
        /// Devuelve el contacto más reciente de un estudiante, o null si
        /// nunca fue contactado.
        /// </summary>
        /// <param name="estudianteId">Estudiante a consultar.</param>
        Contacto? UltimoContacto(Guid estudianteId);

        /// <summary>
        /// Devuelve, para cada estudiante de la comisión que tenga algún
        /// contacto, su contacto más reciente (H11: panel). Los que nunca
        /// fueron contactados no aparecen.
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        Dictionary<Guid, Contacto> UltimoContactoPorEstudiante(string codigoComision);
    }
}
