using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    public class AsistenciaRepository : IAsistenciaRepository
    {
        public void RegistrarAsistencia(Asistencia asistencia)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO asistencias (clase_id, estudiante_id, condicion)
                VALUES (@claseId, @estudianteId, @condicion);
                """;
            comando.Parameters.AddWithValue("@claseId", asistencia.ClaseId);
            comando.Parameters.AddWithValue("@estudianteId", asistencia.EstudianteId);
            comando.Parameters.AddWithValue("@condicion", asistencia.Condicion.ToString());
            comando.ExecuteNonQuery();
        }

        public bool ActualizarCondicion(long claseId, long estudianteId, Condicion condicion)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                UPDATE asistencias
                SET condicion = @condicion
                WHERE clase_id = @claseId AND estudiante_id = @estudianteId;
                """;
            comando.Parameters.AddWithValue("@condicion", condicion.ToString());
            comando.Parameters.AddWithValue("@claseId", claseId);
            comando.Parameters.AddWithValue("@estudianteId", estudianteId);
            return comando.ExecuteNonQuery() > 0;
        }

        public List<Asistencia> ListarPorClase(long claseId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, clase_id, estudiante_id, condicion
                FROM asistencias
                WHERE clase_id = @claseId
                ORDER BY estudiante_id;
                """;
            comando.Parameters.AddWithValue("@claseId", claseId);

            var asistencias = new List<Asistencia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                asistencias.Add(Mapear(lector));
            return asistencias;
        }

        public List<Asistencia> ListarPorEstudiante(long estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, clase_id, estudiante_id, condicion
                FROM asistencias
                WHERE estudiante_id = @estudianteId
                ORDER BY clase_id;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId);

            var asistencias = new List<Asistencia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                asistencias.Add(Mapear(lector));
            return asistencias;
        }

        private static Asistencia Mapear(SqliteDataReader lector) => new Asistencia
        {
            Id = lector.GetInt64(0),
            ClaseId = lector.GetInt64(1),
            EstudianteId = lector.GetInt64(2),
            Condicion = Enum.Parse<Condicion>(lector.GetString(3)),
        };
    }
}
