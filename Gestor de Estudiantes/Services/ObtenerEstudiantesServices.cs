GIT using Gestor_de_Estudiantes.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestor_de_Estudiantes.Services
{
    internal class ObtenerEstudiantesServices
    {
        public ObtenerEstudiantesServices()
        {
        }

        public List<Estudiante> ObtenerEstudiantes()
        {
            return new List<Estudiante>()
            {
                new Estudiante() { Nombre = "Juan", Apellido = "Perez"},
                new Estudiante() { Nombre = "Maria", Apellido = "Gonzalez"},
                new Estudiante() { Nombre = "Pedro", Apellido = "Lopez"},
            };
        }

    }

}
