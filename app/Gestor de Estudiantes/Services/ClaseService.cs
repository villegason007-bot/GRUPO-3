using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada del calendario de clases (H2): lista las clases de una
    /// comisión, da de alta clases nuevas y marca su estado hábil/no hábil.
    /// </summary>
    public class ClaseService : IClaseService
    {
        private readonly IClaseRepository _clases;
        private readonly IComisionRepository _comisiones;

        /// <summary>Crea el servicio con los repositorios de clases y comisiones.</summary>
        public ClaseService(
            IClaseRepository clases,
            IComisionRepository comisiones)
        {
            _clases = clases;
            _comisiones = comisiones;
        }

        /// <summary>Devuelve las clases de la comisión ordenadas por fecha.</summary>
        /// <param name="codigoComision">Código de la comisión (debe existir).</param>
        public List<Clase> ListarClases(string codigoComision)
        {
            ValidarComision(codigoComision);

            return _clases.ListarClases(codigoComision.Trim());
        }

        /// <summary>
        /// Agrega una clase a la comisión, nacida como hábil (H2).
        /// </summary>
        /// <param name="codigoComision">Comisión a la que pertenece la clase.</param>
        /// <param name="fecha">Fecha del encuentro (obligatoria; se guarda sin hora).</param>
        /// <returns>La clase creada.</returns>
        /// <exception cref="ArgumentException">Si falta la fecha.</exception>
        /// <exception cref="InvalidOperationException">Si la comisión no existe.</exception>
        public Clase AgregarClase(
            string codigoComision,
            DateTime fecha)
        {
            ValidarComision(codigoComision);

            if (fecha == default)
            {
                throw new ArgumentException(
                    "La fecha de la clase es obligatoria.");
            }

            // QA002: la misma comisión no puede tener dos clases el mismo día.
            if (_clases.ExisteClaseEnFecha(codigoComision.Trim(), fecha.Date))
            {
                throw new InvalidOperationException(
                    $"Ya hay una clase registrada el {fecha.Date:dd/MM/yyyy} en la comisión {codigoComision.Trim()}.");
            }

            var clase = new Clase
            {
                Fecha = fecha.Date,
                Estado = EstadoClase.Habil
            };

            _clases.AgregarClase(
                clase,
                codigoComision.Trim());

            return clase;
        }

        /// <summary>
        /// Marca una clase como hábil o no hábil (feriado/suspensión, H2).
        /// </summary>
        /// <param name="claseId">Clase a actualizar.</param>
        /// <param name="estado">Nuevo estado.</param>
        /// <returns>true si se actualizó; false si la clase no existe.</returns>
        public bool MarcarEstado(
            Guid claseId,
            EstadoClase estado)
        {
            return _clases.MarcarEstado(claseId, estado);
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
