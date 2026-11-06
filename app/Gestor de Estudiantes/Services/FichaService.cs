using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de la ficha del estudiante (H9): compone los datos
    /// identificatorios y de contacto con el historial de contactos
    /// del tutor (H10).
    /// </summary>
    public class FichaService : IFichaService
    {
        private readonly IEstudianteRepository _estudiantes;
        private readonly IContactoRepository _contactos;

        /// <summary>Crea el servicio con los repositorios de estudiantes y contactos.</summary>
        public FichaService(
            IEstudianteRepository estudiantes,
            IContactoRepository contactos)
        {
            _estudiantes = estudiantes;
            _contactos = contactos;
        }

        /// <summary>Devuelve la ficha de un estudiante activo con sus contactos (H9).</summary>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        public FichaEstudiante ObtenerFicha(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();

            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");

            var estudiante = _estudiantes.BuscarEstudiante(legajo)
                ?? throw new InvalidOperationException(
                    $"No existe el estudiante {legajo}.");

            return new FichaEstudiante
            {
                Id = estudiante.Id,
                Legajo = estudiante.Legajo,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                Telefono = estudiante.Telefono,
                FechaIncorporacion = estudiante.FechaIncorporacion,
                CodigoComision = estudiante.CodigoComision,
                Contactos = _contactos.ListarContactos(estudiante.Id),
            };
        }
    }
}
