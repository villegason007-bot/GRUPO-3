using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada del calendario unificado (H2): expone la consulta de
    /// clases y trabajos unidos por fecha, hoy en
    /// <see cref="CalendarioRepository"/>.
    /// </summary>
    public class CalendarioService : ICalendarioService
    {
        private readonly ICalendarioRepository _calendario;

        /// <summary>Crea el servicio con su repositorio de calendario.</summary>
        public CalendarioService(ICalendarioRepository calendario)
        {
            _calendario = calendario;
        }

        /// <summary>
        /// Devuelve el calendario de la comisión: clases y trabajos unidos
        /// por fecha (H2, Escenario 3).
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        public List<ElementoCalendario> VerCalendario(string codigoComision)
            => _calendario.VerCalendario(codigoComision);
    }
}
