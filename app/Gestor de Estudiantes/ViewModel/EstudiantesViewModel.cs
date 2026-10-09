using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;
using System.Collections.ObjectModel;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class EstudiantesViewModel
    {
        private readonly EstudianteService _service;

        public ObservableCollection<Estudiante> ListadoEstudiantes { get; } = new();

        public EstudiantesViewModel(EstudianteService service)
        {
            _service = service;
        }

        public void MostrarEstudiantes(string codigoComision)
        {
            ListadoEstudiantes.Clear();
            foreach (var est in _service.ListarEstudiantes(codigoComision))
                ListadoEstudiantes.Add(est);
        }

        public const string ComisionActual = "1K1"; // provisorio, igual que en la lista

        public void MostrarEstudiantes() => MostrarEstudiantes(ComisionActual);

        public void AgregarEstudiante(string legajo, string nombre)
        {
            _service.AgregarEstudiante(legajo, nombre, ComisionActual);
            MostrarEstudiantes(ComisionActual);
        }
    }
}