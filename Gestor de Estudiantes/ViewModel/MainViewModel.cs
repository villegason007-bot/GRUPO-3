using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestor_de_Estudiantes.ViewModel
{
    internal class MainViewModel
    {
        public ObservableCollection<Estudiante> ListadoEstudiantes { get ; set; }

        public MainViewModel() 
        {
            this.ListadoEstudiantes = new ObservableCollection<Estudiante>();
        }
        public void MostrarEstudiantes()
        {
            ObtenerEstudiantesServices services = new ObtenerEstudiantesServices();
            

            foreach (Estudiante est in services.ObtenerEstudiantes())
            {
                this.ListadoEstudiantes.Add(est);
            }
        }
    }
}
