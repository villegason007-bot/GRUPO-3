using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    public class EstudianteRepository : IEstudianteRepository
    {
        public List<Estudiante> ListarEstudiantes(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT e.id, e.legajo, e.nombre, e.apellido, e.telefono, e.fecha_incorporacion
                FROM estudiantes e
                INNER JOIN comisiones c ON c.id = e.comision_id
                WHERE c.codigo = @codigo
                ORDER BY e.legajo;
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var estudiantes = new List<Estudiante>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                estudiantes.Add(Mapear(lector));
            return estudiantes;
        }

        public Estudiante? BuscarEstudiante(string legajo)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT e.id, e.legajo, e.nombre, e.apellido, e.telefono, e.fecha_incorporacion
                FROM estudiantes e
                WHERE e.legajo = @legajo;
                """;
            comando.Parameters.AddWithValue("@legajo", legajo);

            using var lector = comando.ExecuteReader();
            return lector.Read() ? Mapear(lector) : null;
        }

        public void AgregarEstudiante(Estudiante estudiante, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO estudiantes (id, legajo, nombre, apellido, comision_id)
                VALUES (@id, @legajo, @nombre, @apellido,
                        (SELECT id FROM comisiones WHERE codigo = @codigo));
                """;
            comando.Parameters.AddWithValue("@id", estudiante.Id.ToString("D"));
            comando.Parameters.AddWithValue("@legajo", estudiante.Legajo);
            comando.Parameters.AddWithValue("@nombre", estudiante.Nombre);
            comando.Parameters.AddWithValue("@apellido", estudiante.Apellido);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.ExecuteNonQuery();
        }

        public bool EliminarEstudiante(string legajo)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "DELETE FROM estudiantes WHERE legajo = @legajo;";
            comando.Parameters.AddWithValue("@legajo", legajo);
            return comando.ExecuteNonQuery() > 0;
        }

        private static Estudiante Mapear(SqliteDataReader lector) => new Estudiante
        {
            Id = Guid.Parse(lector.GetString(0)),
            Legajo = lector.GetString(1),
            Nombre = lector.GetString(2),
            Apellido = lector.IsDBNull(3) ? string.Empty : lector.GetString(3),
            Telefono = lector.IsDBNull(4) ? null : lector.GetString(4),
            FechaIncorporacion = lector.IsDBNull(5) ? null : lector.GetString(5),
        };
    }
}
