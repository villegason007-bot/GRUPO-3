using Gestor_de_Estudiantes.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestor_de_Estudiantes.Services
{
    internal class ObtenerComisionesServices
    {
        public ObtenerComisionesServices()
        {
        }

        public List<Comisiones> ObtenerComisiones()
        {
            return new List<Comisiones>()
            {
                new Comisiones() { Nombre = "Comision 1"},
                new Comisiones() { Nombre = "Comision 2"},
                new Comisiones() { Nombre = "Comision 3"},
            };
        }
    }
}
