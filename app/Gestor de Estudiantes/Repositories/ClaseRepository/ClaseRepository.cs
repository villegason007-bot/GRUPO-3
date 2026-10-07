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
                SELECT c.id, c.fecha
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
                INSERT INTO clases (fecha, comision_id)
                VALUES (@fecha,
                        (SELECT id FROM comisiones WHERE codigo = @codigo));
                """;
            comando.Parameters.AddWithValue("@fecha", clase.Fecha);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.ExecuteNonQuery();
        }

        private static Clase Mapear(SqliteDataReader lector) => new Clase
        {
            Id = lector.GetInt64(0),
            Fecha = lector.GetDateTime(1),
        };
    }
}
