using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;
using System.Collections.ObjectModel;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class ComisionesViewModel
    {
        private readonly ComisionService _service;

        public ObservableCollection<Comision> ListadoComisiones { get; } = new();

        public ComisionesViewModel(ComisionService service)
        {
            _service = service;
        }

        public void MostrarComisiones()
        {
            ListadoComisiones.Clear();
            foreach (var c in _service.ListarComisiones())
                ListadoComisiones.Add(c);
        }

        public void AgregarComision(string codigo, int semanas)
        {
            _service.AgregarComision(codigo, semanas);
            MostrarComisiones();
        }
    }
}