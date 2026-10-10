using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    public class ClaseRepository : IClaseRepository
    {
        public List<Clase> ListarClases(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT c.id, c.fecha, c.estado
                FROM clases c
                INNER JOIN comisiones com ON com.id = c.comision_id
                WHERE com.codigo = @codigo
                ORDER BY c.fecha;
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var clases = new List<Clase>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                clases.Add(Mapear(lector));
            return clases;
        }

        public void AgregarClase(Clase clase, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO clases (id, fecha, estado, comision_id)
                VALUES (@id, @fecha, @estado,
                        (SELECT id FROM comisiones WHERE codigo = @codigo));
                """;
            comando.Parameters.AddWithValue("@id", clase.Id.ToString("D"));
            comando.Parameters.AddWithValue("@fecha", clase.Fecha);
            comando.Parameters.AddWithValue("@estado", clase.Estado.ToString());
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.ExecuteNonQuery();
        }

        public bool MarcarEstado(Guid claseId, EstadoClase estado)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "UPDATE clases SET estado = @estado WHERE id = @id;";
            comando.Parameters.AddWithValue("@estado", estado.ToString());
            comando.Parameters.AddWithValue("@id", claseId.ToString("D"));
            return comando.ExecuteNonQuery() > 0;
        }

        internal static EstadoClase EstadoDe(SqliteConnection conexion, Guid claseId)
        {
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT estado FROM clases WHERE id = @id;";
            comando.Parameters.AddWithValue("@id", claseId.ToString("D"));
            var estado = comando.ExecuteScalar() as string;
            return estado is null ? EstadoClase.Habil : Enum.Parse<EstadoClase>(estado);
        }

        private static Clase Mapear(SqliteDataReader lector) => new Clase
        {
            Id = Guid.Parse(lector.GetString(0)),
            Fecha = lector.GetDateTime(1),
            Estado = Enum.Parse<EstadoClase>(lector.GetString(2)),
        };
    }
}
