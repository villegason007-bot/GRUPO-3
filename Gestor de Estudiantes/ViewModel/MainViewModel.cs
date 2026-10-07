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
        public ObservableCollection<Comisiones> ListadoComisiones { get ; set; }
        public ObservableCollection<Trabajo> ListadoTrabajos { get; set; }

        public MainViewModel() 
        {
            this.ListadoEstudiantes = new ObservableCollection<Estudiante>();
            this.ListadoComisiones = new ObservableCollection<Comisiones>();
            this.ListadoTrabajos = new ObservableCollection<Trabajo>();
        }
        public void MostrarEstudiantes()
        {
            ObtenerEstudiantesServices services = new ObtenerEstudiantesServices();
            

            foreach (Estudiante est in services.ObtenerEstudiantes())
            {
                this.ListadoEstudiantes.Add(est);
            }
        }

        public void MostrarComisiones()
        {
            ObtenerComisionesServices services = new ObtenerComisionesServices();

            foreach (Comisiones com in services.ObtenerComisiones())
            {
                this.ListadoComisiones.Add(com);
            }
        }

        public void MostrarTrabajos()
        {
            ObtenerTrabajosServices services = new ObtenerTrabajosServices();
            foreach (Trabajo tr in services.ObtenerTrabajos())
            {
                this.ListadoTrabajos.Add(tr);
            }
        }
    }
}
