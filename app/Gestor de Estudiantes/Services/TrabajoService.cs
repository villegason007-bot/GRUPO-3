
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class TrabajoService
    {
        private readonly ITrabajoRepository _trabajos;
        private readonly IComisionRepository _comisiones;

        public TrabajoService(
            ITrabajoRepository trabajos,
            IComisionRepository comisiones)
        {
            _trabajos = trabajos;
            _comisiones = comisiones;
        }

        public List<Trabajo> ListarTrabajos(string codigoComision)
        {
            ValidarComision(codigoComision);

            return _trabajos.ListarTrabajos(codigoComision.Trim());
        }

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
