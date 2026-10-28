using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Arma la lista de eventos del calendario a partir de las clases y
    /// los trabajos de una comisión, ordenada por fecha (en la misma
    /// fecha, las clases antes que los trabajos).
    /// </summary>
    public class ConstructorDeEventosDeCalendario
    {
        /// <summary>
        /// Convierte clases y trabajos en eventos de calendario ordenados.
        /// </summary>
        /// <param name="clases">Clases de la comisión.</param>
        /// <param name="trabajos">Trabajos de la comisión.</param>
        /// <returns>Los eventos de ambas fuentes, ordenados por fecha.</returns>
        public List<EventoCalendario> Construir(
            List<Clase> clases,
            List<Trabajo> trabajos)
        {
            var eventos = new List<EventoCalendario>();

            foreach (var clase in clases)
            {
                eventos.Add(new EventoCalendario
                {
                    Fecha = clase.Fecha,
                    Tipo = "CLASE",
                    Titulo = "Clase",
                    Detalle = $"Estado: {clase.Estado}"
                });
            }

            foreach (var trabajo in trabajos)
            {
                eventos.Add(new EventoCalendario
                {
                    Fecha = trabajo.FechaDeEntrega,
                    Tipo = "TRABAJO",
                    Titulo = trabajo.Titulo,
                    Detalle = "Fecha de entrega"
                });
            }

            return eventos
                .OrderBy(e => e.Fecha)
                .ThenBy(e => e.Tipo)
                .ToList();
        }
    }
}
