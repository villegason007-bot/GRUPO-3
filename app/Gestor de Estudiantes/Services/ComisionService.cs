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

        public Comision AgregarComision(string codigo)
        {
            codigo = (codigo ?? string.Empty).Trim();

            if (codigo.Length == 0)
                throw new ArgumentException("El código de comisión es obligatorio.");
            if (_comisiones.ExisteComision(codigo))
                throw new InvalidOperationException($"Ya existe la comisión {codigo}.");

            var comision = new Comision { Codigo = codigo };
            _comisiones.AgregarComision(comision);
            return comision;
        }
    }
}
