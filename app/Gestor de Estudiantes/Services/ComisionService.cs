using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class ComisionService
    {
        private readonly IComisionRepository _comisiones;

        public ComisionService(IComisionRepository comisiones)
        {
            _comisiones = comisiones;
        }

        public List<Comision> ListarComisiones()
            => _comisiones.ListarComisiones();

        public List<ElementoCalendario> VerCalendario(string codigoComision)
            => _comisiones.VerCalendario(codigoComision);

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
