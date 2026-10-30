using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de contactos (H10/H11): registrar un
    /// contacto con su resultado y consultar quién y cuándo se contactó.
    /// </summary>
    public interface IContactoService
    {
        /// <summary>
        /// Registra un contacto con la fecha del día (H10).
        /// </summary>
        /// <param name="legajo">Estudiante contactado (debe existir y estar activo).</param>
        /// <param name="medio">Medio utilizado (llamada o mensaje).</param>
        /// <param name="resultado">Resultado obtenido (contestó, quedó en veremos o sin respuesta).</param>
        /// <returns>El contacto registrado.</returns>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        Contacto RegistrarContacto(string legajo, MedioContacto medio, ResultadoContacto resultado);

        /// <summary>Devuelve los contactos de un estudiante, del más reciente al más antiguo.</summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        List<Contacto> ListarContactos(string legajo);

        /// <summary>
        /// Devuelve el último contacto de un estudiante, o null si nunca
        /// fue contactado (H11: «sin contactar»).
        /// </summary>
        /// <param name="legajo">Legajo del estudiante (debe existir).</param>
        Contacto? UltimoContacto(string legajo);

        /// <summary>
        /// Devuelve, por estudiante, su último contacto para el panel de
        /// riesgo (H11). Los nunca contactados no aparecen en el mapa.
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        Dictionary<Guid, Contacto> UltimoContactoPorEstudiante(string codigoComision);
    }
}
