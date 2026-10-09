using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Test
{
    public abstract class RepositorioTestBase
    {
        [TestInitialize]
        public void CrearBaseDeDatos()
        {
            BaseDeDatos.Ruta = Path.Combine(Path.GetTempPath(), $"ppii_test_{Guid.NewGuid():N}.db");
            BaseDeDatos.Inicializar();
        }

        [TestCleanup]
        public void EliminarBaseDeDatos()
        {
            if (File.Exists(BaseDeDatos.Ruta))
                File.Delete(BaseDeDatos.Ruta);
        }

        protected static Guid CrearClase()
        {
            var repositorio = new ClaseRepository();
            repositorio.AgregarClase(new Clase { Fecha = new DateTime(2026, 10, 5) }, "1K1");
            return repositorio.ListarClases("1K1")[0].Id;
        }

        protected static Guid CrearEstudiante()
        {
            return CrearEstudiante("9001", "Ana");
        }

        protected static Guid CrearEstudiante(string legajo, string nombre)
        {
            var repositorio = new EstudianteRepository();
            repositorio.AgregarEstudiante(new Estudiante { Legajo = legajo, Nombre = nombre, Apellido = "García" }, "1K1");
            return repositorio.ListarEstudiantes("1K1").First(estudiante => estudiante.Legajo == legajo).Id;
        }

        protected static Guid CrearTrabajo()
        {
            var repositorio = new TrabajoRepository();
            repositorio.AgregarTrabajo(new Trabajo { Titulo = "TP1", FechaDeEntrega = new DateTime(2026, 10, 20) }, "1K1");
            return repositorio.ListarTrabajos("1K1")[0].Id;
        }
    }
}
