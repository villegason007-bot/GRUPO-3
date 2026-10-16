using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de comisiones (H1/H2): alta de comisiones, listado y
    /// consulta del calendario unificado de la comisión.
    /// </summary>
    public class ComisionService
    {
        private readonly IComisionRepository _comisiones;

        /// <summary>Crea el servicio con su repositorio de comisiones.</summary>
        public ComisionService(IComisionRepository comisiones)
        {
            _comisiones = comisiones;
        }

        /// <summary>Devuelve todas las comisiones ordenadas por código.</summary>
        public List<Comision> ListarComisiones()
            => _comisiones.ListarComisiones();

        /// <summary>
        /// Devuelve el calendario de la comisión: clases y trabajos unidos
        /// por fecha (H2, Escenario 3).
        /// </summary>
        /// <param name="codigoComision">Código de la comisión.</param>
        public List<ElementoCalendario> VerCalendario(string codigoComision)
            => _comisiones.VerCalendario(codigoComision);

        /// <summary>
        /// Crea una comisión validando código y semanas (H1).
        /// </summary>
        /// <param name="codigo">Código visible (obligatorio, único).</param>
        /// <param name="semanas">Cantidad de semanas del cuatrimestre; por defecto 16.</param>
        /// <returns>La comisión creada.</returns>
        /// <exception cref="ArgumentException">Si el código está vacío o las semanas son menores a 1.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una comisión con ese código.</exception>
        public Comision AgregarComision(string codigo, int semanas = 16)
        {
            codigo = (codigo ?? string.Empty).Trim();

            if (codigo.Length == 0)
                throw new ArgumentException("El código de comisión es obligatorio.");
            if (semanas < 1)
                throw new ArgumentException("La cantidad de semanas debe ser al menos 1.");
            if (_comisiones.ExisteComision(codigo))
                throw new InvalidOperationException($"Ya existe la comisión {codigo}.");

            var comision = new Comision { Codigo = codigo, Semanas = semanas };
            _comisiones.AgregarComision(comision);
            return comision;
        }
    }
}
