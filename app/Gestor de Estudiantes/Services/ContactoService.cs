using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de contactos (H10/H11): registra contacto con la fecha
    /// del día y consulta el último contacto por estudiante o por
    /// comisión.
    /// </summary>
    public class ContactoService : IContactoService
    {
        private readonly IContactoRepository _contactos;
        private readonly IEstudianteRepository _estudiantes;

        /// <summary>Crea el servicio con los repositorios de contactos y estudiantes.</summary>
        public ContactoService(
            IContactoRepository contactos,
            IEstudianteRepository estudiantes)
        {
            _contactos = contactos;
            _estudiantes = estudiantes;
        }

        /// <summary>Registra un contacto con la fecha del día (H10).</summary>
        /// <exception cref="ArgumentException">Si el legajo está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si no existe el estudiante.</exception>
        public Contacto RegistrarContacto(
            string legajo,
            MedioContacto medio,
            ResultadoContacto resultado)
        {
            var estudiante = BuscarEstudiante(legajo);

            var contacto = new Contacto
            {
                EstudianteId = estudiante.Id,
                Fecha = DateTime.Today,
                Medio = medio,
                Resultado = resultado,
            };

            _contactos.RegistrarContacto(contacto);
            return contacto;
        }

        /// <summary>Devuelve los contactos de un estudiante, del más reciente al más antiguo.</summary>
        public List<Contacto> ListarContactos(string legajo)
        {
            var estudiante = BuscarEstudiante(legajo);
            return _contactos.ListarContactos(estudiante.Id);
        }

        /// <summary>Devuelve el último contacto de un estudiante, o null si nunca fue contactado.</summary>
        public Contacto? UltimoContacto(string legajo)
        {
            var estudiante = BuscarEstudiante(legajo);
            return _contactos.UltimoContacto(estudiante.Id);
        }

        /// <summary>Devuelve el último contacto de cada estudiante contactado de la comisión (H11).</summary>
        public Dictionary<Guid, Contacto> UltimoContactoPorEstudiante(string codigoComision)
            => _contactos.UltimoContactoPorEstudiante(
                (codigoComision ?? string.Empty).Trim());

        /// <summary>Valida el legajo y devuelve el estudiante activo correspondiente.</summary>
        private Estudiante BuscarEstudiante(string legajo)
        {
            legajo = (legajo ?? string.Empty).Trim();

            if (legajo.Length == 0)
                throw new ArgumentException("El legajo es obligatorio.");

            return _estudiantes.BuscarEstudiante(legajo)
                ?? throw new InvalidOperationException(
                    $"No existe el estudiante {legajo}.");
        }
    }
}
