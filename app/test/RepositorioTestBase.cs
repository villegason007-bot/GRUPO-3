using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Microsoft.Data.Sqlite;

namespace Test
{
    public abstract class RepositorioTestBase
    {
        [TestInitialize]
        public void CrearBaseDeDatos()
        {
            BaseDeDatos.Ruta = Path.Combine(Path.GetTempPath(), $"ppii_test_{Guid.NewGuid():N}.db");
            BaseDeDatos.Inicializar();
            LimpiarDatosSembrados();
        }

        /// <summary>
        /// Inicializar() siembra comisiones y estudiantes de ejemplo para la app.
        /// Los tests necesitan partir de una base vacía con la comisión 1K1.
        /// </summary>
        private static void LimpiarDatosSembrados()
        {
            using (var conexion = BaseDeDatos.Abrir())
            {
                using var comando = conexion.CreateCommand();
                comando.CommandText = """
                    DELETE FROM contactos;
                    DELETE FROM entregas;
                    DELETE FROM asistencias;
                    DELETE FROM estudiante_materia;
                    DELETE FROM materia_comision;
                    DELETE FROM trabajos;
                    DELETE FROM clases;
                    DELETE FROM estudiantes;
                    DELETE FROM materias;
                    DELETE FROM comisiones;
                    """;
                comando.ExecuteNonQuery();
            }

            new ComisionRepository().AgregarComision(new Comision { Codigo = "1K1" });
        }

        [TestCleanup]
        public void EliminarBaseDeDatos()
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(BaseDeDatos.Ruta))
                File.Delete(BaseDeDatos.Ruta);
        }

        protected static Guid CrearClase()
        {
            return CrearClase(new DateTime(2026, 10, 5));
        }

        protected static Guid CrearClase(DateTime fecha)
        {
            var repositorio = new ClaseRepository();
            repositorio.AgregarClase(new Clase { Fecha = fecha }, "1K1");
            return repositorio.ListarClases("1K1").First(clase => clase.Fecha == fecha).Id;
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
            // Fecha relativa (una semana atrás): un trabajo ya vencido,
            // sin depender del calendario real al correr la suite.
            return CrearTrabajo(DateTime.Today.AddDays(-7));
        }

        protected static Guid CrearTrabajo(DateTime fechaDeEntrega)
        {
            var repositorio = new TrabajoRepository();
            repositorio.AgregarTrabajo(
                new Trabajo { Titulo = "TP1", FechaDeEntrega = fechaDeEntrega }, "1K1");
            return repositorio.ListarTrabajos("1K1")[0].Id;
        }
    }
}