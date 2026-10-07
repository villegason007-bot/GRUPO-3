using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    public class TrabajoRepository : ITrabajoRepository
    {
        public List<Trabajo> ListarTrabajos(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT t.id, t.titulo, t.fecha_entrega
                FROM trabajos t
                INNER JOIN comisiones com ON com.id = t.comision_id
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

        private static Trabajo Mapear(SqliteDataReader lector) => new Trabajo
        {
            Id = Guid.Parse(lector.GetString(0)),
            Titulo = lector.GetString(1),
            FechaDeEntrega = lector.GetDateTime(2),
        };
    }
}
