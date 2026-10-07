using Gestor_de_Estudiantes.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestor_de_Estudiantes.Services
{
    internal class ObtenerTrabajosServices
    {
        public ObtenerTrabajosServices()
        {
        }

        public List<Trabajo> ObtenerTrabajos()
        {
            return new List<Trabajo>()
            {
                new Trabajo() { Titulo = "Trabajo 1", FechaDeEntrega = new DateTime(2023, 5, 15)},
                new Trabajo() { Titulo = "Trabajo 2", FechaDeEntrega = new DateTime(2023, 5, 15)},
                new Trabajo() { Titulo = "Trabajo 3", FechaDeEntrega = new DateTime(2023, 5, 15)},
            };
        }
    }
}
