using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    public class EntregaRepository : IEntregaRepository
    {
        public void RegistrarEntrega(Entrega entrega)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO entregas (trabajo_id, estudiante_id, entregado, fecha)
                VALUES (@trabajoId, @estudianteId, @entregado, @fecha);
                """;
            comando.Parameters.AddWithValue("@trabajoId", entrega.TrabajoId);
            comando.Parameters.AddWithValue("@estudianteId", entrega.EstudianteId);
            comando.Parameters.AddWithValue("@entregado", entrega.Entregado);
            comando.Parameters.AddWithValue("@fecha", (object?)entrega.Fecha ?? DBNull.Value);
            comando.ExecuteNonQuery();
        }

        public bool ActualizarEntrega(long trabajoId, long estudianteId, bool entregado, DateTime? fecha)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                UPDATE entregas
                SET entregado = @entregado, fecha = @fecha
                WHERE trabajo_id = @trabajoId AND estudiante_id = @estudianteId;
                """;
            comando.Parameters.AddWithValue("@entregado", entregado);
            comando.Parameters.AddWithValue("@fecha", (object?)fecha ?? DBNull.Value);
            comando.Parameters.AddWithValue("@trabajoId", trabajoId);
            comando.Parameters.AddWithValue("@estudianteId", estudianteId);
            return comando.ExecuteNonQuery() > 0;
        }

        public List<Entrega> ListarPorTrabajo(long trabajoId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, trabajo_id, estudiante_id, entregado, fecha
                FROM entregas
                WHERE trabajo_id = @trabajoId
                ORDER BY estudiante_id;
                """;
            comando.Parameters.AddWithValue("@trabajoId", trabajoId);

            var entregas = new List<Entrega>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                entregas.Add(Mapear(lector));
            return entregas;
        }

        public List<Entrega> ListarPorEstudiante(long estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, trabajo_id, estudiante_id, entregado, fecha
                FROM entregas
                WHERE estudiante_id = @estudianteId
                ORDER BY trabajo_id;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId);

            var entregas = new List<Entrega>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                entregas.Add(Mapear(lector));
            return entregas;
        }

        private static Entrega Mapear(SqliteDataReader lector) => new Entrega
        {
            Id = lector.GetInt64(0),
            TrabajoId = lector.GetInt64(1),
            EstudianteId = lector.GetInt64(2),
            Entregado = lector.GetInt64(3) != 0,
            Fecha = lector.IsDBNull(4) ? null : lector.GetDateTime(4),
        };
    }
}
