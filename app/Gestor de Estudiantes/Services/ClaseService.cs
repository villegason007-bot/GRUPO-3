
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class ClaseService
    {
        private readonly IClaseRepository _clases;
        private readonly IComisionRepository _comisiones;

        public ClaseService(
            IClaseRepository clases,
            IComisionRepository comisiones)
        {
            _clases = clases;
            _comisiones = comisiones;
        }

        public List<Clase> ListarClases(string codigoComision)
        {
            ValidarComision(codigoComision);

            return _clases.ListarClases(codigoComision.Trim());
        }

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

        public bool MarcarEstado(
            Guid claseId,
            EstadoClase estado)
        {
            return _clases.MarcarEstado(claseId, estado);
        }

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
