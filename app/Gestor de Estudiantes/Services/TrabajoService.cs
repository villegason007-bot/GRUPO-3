using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de trabajos prácticos (H5): lista los trabajos de una
    /// comisión y da de alta trabajos nuevos con su fecha de entrega.
    /// </summary>
    public class TrabajoService : ITrabajoService
    {
        private readonly ITrabajoRepository _trabajos;
        private readonly IComisionRepository _comisiones;

        /// <summary>Crea el servicio con los repositorios de trabajos y comisiones.</summary>
        public TrabajoService(
            ITrabajoRepository trabajos,
            IComisionRepository comisiones)
        {
            _trabajos = trabajos;
            _comisiones = comisiones;
        }

        /// <summary>Devuelve los trabajos de la comisión ordenados por fecha de entrega.</summary>
        /// <param name="codigoComision">Código de la comisión (debe existir).</param>
        public List<Trabajo> ListarTrabajos(string codigoComision)
        {
            ValidarComision(codigoComision);

            return _trabajos.ListarTrabajos(codigoComision.Trim());
        }

        /// <summary>
        /// Agrega un trabajo a la comisión (H5).
        /// </summary>
        /// <param name="codigoComision">Comisión a la que pertenece el trabajo.</param>
        /// <param name="titulo">Título del trabajo (obligatorio).</param>
        /// <param name="fechaDeEntrega">Fecha límite de entrega (obligatoria; se guarda sin hora).</param>
        /// <returns>El trabajo creado.</returns>
        /// <exception cref="ArgumentException">Si falta el título o la fecha.</exception>
        /// <exception cref="InvalidOperationException">Si la comisión no existe.</exception>
        public Trabajo AgregarTrabajo(
            string codigoComision,
            string titulo,
            DateTime fechaDeEntrega)
        {
            ValidarComision(codigoComision);

            titulo = (titulo ?? string.Empty).Trim();

            if (titulo.Length == 0)
            {
                throw new ArgumentException(
                    "El título del trabajo es obligatorio.");
            }

            if (fechaDeEntrega == default)
            {
                throw new ArgumentException(
                    "La fecha de entrega es obligatoria.");
            }

            var trabajo = new Trabajo
            {
                Titulo = titulo,
                FechaDeEntrega = fechaDeEntrega.Date
            };

            _trabajos.AgregarTrabajo(
                trabajo,
                codigoComision.Trim());

            return trabajo;
        }

        /// <summary>Valida que se indique una comisión existente (ignorando mayúsculas/minúsculas y espacios).</summary>
        private void ValidarComision(string codigoComision)
        {
            codigoComision = (codigoComision ?? string.Empty).Trim();

            if (codigoComision.Length == 0)
            {
                throw new ArgumentException(
                    "Seleccioná una comisión.");
            }

            bool existe = _comisiones.ListarComisiones().Any(c =>
                string.Equals(
                    c.Codigo,
                    codigoComision,
                    StringComparison.OrdinalIgnoreCase));

            if (!existe)
            {
                throw new InvalidOperationException(
                    $"No existe la comisión {codigoComision}.");
            }
        }
    }
}
