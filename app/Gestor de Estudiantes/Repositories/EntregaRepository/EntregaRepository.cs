using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de entregas de trabajos sobre SQLite (H5).
    /// </summary>
    public class EntregaRepository : IEntregaRepository
    {
        /// <summary>Inserta un registro de entrega nuevo.</summary>
        public void RegistrarEntrega(Entrega entrega)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO entregas (id, trabajo_id, estudiante_id, entregado, fecha)
                VALUES (@id, @trabajoId, @estudianteId, @entregado, @fecha);
                """;
            comando.Parameters.AddWithValue("@id", entrega.Id.ToString("D"));
            comando.Parameters.AddWithValue("@trabajoId", entrega.TrabajoId.ToString("D"));
            comando.Parameters.AddWithValue("@estudianteId", entrega.EstudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@entregado", entrega.Entregado);
            comando.Parameters.AddWithValue("@fecha", (object?)entrega.Fecha ?? DBNull.Value);
            comando.ExecuteNonQuery();
        }

        /// <summary>Actualiza entregado y fecha de un registro existente (la fecha queda null si no entregó).</summary>
        /// <returns>true si había registro y se actualizó.</returns>
        public bool ActualizarEntrega(Guid trabajoId, Guid estudianteId, bool entregado, DateTime? fecha)
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
            comando.Parameters.AddWithValue("@trabajoId", trabajoId.ToString("D"));
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            return comando.ExecuteNonQuery() > 0;
        }

        /// <summary>Devuelve los registros de entrega de un trabajo, ordenados por estudiante.</summary>
        public List<Entrega> ListarPorTrabajo(Guid trabajoId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, trabajo_id, estudiante_id, entregado, fecha
                FROM entregas
                WHERE trabajo_id = @trabajoId
                ORDER BY estudiante_id;
                """;
            comando.Parameters.AddWithValue("@trabajoId", trabajoId.ToString("D"));

            var entregas = new List<Entrega>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                entregas.Add(Mapear(lector));
            return entregas;
        }

        /// <summary>Devuelve los registros de entrega de un estudiante, ordenados por trabajo.</summary>
        public List<Entrega> ListarPorEstudiante(Guid estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, trabajo_id, estudiante_id, entregado, fecha
                FROM entregas
                WHERE estudiante_id = @estudianteId
                ORDER BY trabajo_id;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            var entregas = new List<Entrega>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                entregas.Add(Mapear(lector));
            return entregas;
        }

        /// <summary>
        /// Devuelve el historial de entregas del estudiante (H13): todos
        /// los trabajos de su comisión con su estado de entrega.
        /// </summary>
        public List<(string Titulo, DateTime FechaDeEntrega, bool Entregado, DateTime? Fecha)> ListarHistorialDeEstudiante(Guid estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT t.titulo, t.fecha_entrega,
                       COALESCE(en.entregado, 0), en.fecha
                FROM trabajos t
                INNER JOIN comisiones com ON com.id = t.comision_id
                INNER JOIN estudiantes e ON e.comision_id = com.id
                LEFT JOIN entregas en ON en.trabajo_id = t.id AND en.estudiante_id = e.id
                WHERE e.id = @estudianteId
                ORDER BY t.fecha_entrega;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            var historial = new List<(string Titulo, DateTime FechaDeEntrega, bool Entregado, DateTime? Fecha)>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                historial.Add((
                    lector.GetString(0),
                    lector.GetDateTime(1),
                    lector.GetInt64(2) != 0,
                    lector.IsDBNull(3) ? null : lector.GetDateTime(3)));
            return historial;
        }

        private static Entrega Mapear(SqliteDataReader lector) => new Entrega
        {
            Id = Guid.Parse(lector.GetString(0)),
            TrabajoId = Guid.Parse(lector.GetString(1)),
            EstudianteId = Guid.Parse(lector.GetString(2)),
            Entregado = lector.GetInt64(3) != 0,
            Fecha = lector.IsDBNull(4) ? null : lector.GetDateTime(4),
        };
    }
}
