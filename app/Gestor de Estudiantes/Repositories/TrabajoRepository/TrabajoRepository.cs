using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de trabajos prácticos sobre SQLite (H5).
    /// </summary>
    public class TrabajoRepository : ITrabajoRepository
    {
        /// <summary>Devuelve los trabajos de la comisión, ordenados por fecha de entrega.</summary>
        public List<Trabajo> ListarTrabajos(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT t.id, t.titulo, t.fecha_entrega, t.materia_id, COALESCE(m.nombre, '')
                FROM trabajos t
                INNER JOIN comisiones com ON com.id = t.comision_id
                LEFT JOIN materias m ON m.id = t.materia_id
                WHERE com.codigo = @codigo
                ORDER BY t.fecha_entrega;
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var trabajos = new List<Trabajo>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                trabajos.Add(Mapear(lector));
            return trabajos;
        }

        /// <summary>Inserta un trabajo nuevo en la comisión indicada por código.</summary>
        public void AgregarTrabajo(Trabajo trabajo, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO trabajos (id, titulo, fecha_entrega, comision_id)
                VALUES (@id, @titulo, @fechaEntrega,
                        (SELECT id FROM comisiones WHERE codigo = @codigo));
                """;
            comando.Parameters.AddWithValue("@id", trabajo.Id.ToString("D"));
            comando.Parameters.AddWithValue("@titulo", trabajo.Titulo);
            comando.Parameters.AddWithValue("@fechaEntrega", trabajo.FechaDeEntrega);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.ExecuteNonQuery();
        }

        public void AgregarTrabajo(Trabajo trabajo, string codigoComision, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO trabajos (id, titulo, fecha_entrega, comision_id, materia_id)
                VALUES (@id, @titulo, @fechaEntrega,
                        (SELECT id FROM comisiones WHERE codigo = @codigo), @materiaId);
                """;
            comando.Parameters.AddWithValue("@id", trabajo.Id.ToString("D"));
            comando.Parameters.AddWithValue("@titulo", trabajo.Titulo);
            comando.Parameters.AddWithValue("@fechaEntrega", trabajo.FechaDeEntrega);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));
            comando.ExecuteNonQuery();
        }

        public void ActualizarMateria(Guid trabajoId, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "UPDATE trabajos SET materia_id = @materiaId WHERE id = @id;";
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));
            comando.Parameters.AddWithValue("@id", trabajoId.ToString("D"));
            comando.ExecuteNonQuery();
        }

        private static Trabajo Mapear(SqliteDataReader lector) => new Trabajo
        {
            Id = Guid.Parse(lector.GetString(0)),
            Titulo = lector.GetString(1),
            FechaDeEntrega = lector.GetDateTime(2),
            MateriaId = lector.IsDBNull(3) ? null : Guid.Parse(lector.GetString(3)),
            NombreMateria = lector.GetString(4),
        };
    }
}
