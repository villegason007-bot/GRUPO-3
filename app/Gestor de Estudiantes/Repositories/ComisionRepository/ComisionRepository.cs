using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de comisiones y de su calendario unificado sobre
    /// SQLite (H1/H2).
    /// </summary>
    public class ComisionRepository : IComisionRepository
    {
        /// <summary>Devuelve todas las comisiones, ordenadas por código.</summary>
        public List<Comision> ListarComisiones()
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT id, codigo, semanas FROM comisiones ORDER BY codigo;";

            var comisiones = new List<Comision>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                comisiones.Add(new Comision
                {
                    Id = Guid.Parse(lector.GetString(0)),
                    Codigo = lector.GetString(1),
                    Semanas = lector.GetInt32(2),
                });
            return comisiones;
        }

        /// <summary>Inserta una comisión nueva; el código debe ser único.</summary>
        public void AgregarComision(Comision comision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "INSERT INTO comisiones (id, codigo, semanas) VALUES (@id, @codigo, @semanas);";
            comando.Parameters.AddWithValue("@id", comision.Id.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", comision.Codigo);
            comando.Parameters.AddWithValue("@semanas", comision.Semanas);
            comando.ExecuteNonQuery();
        }

        /// <summary>Indica si ya existe una comisión con ese código.</summary>
        public bool ExisteComision(string codigo)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT COUNT(*) FROM comisiones WHERE codigo = @codigo;";
            comando.Parameters.AddWithValue("@codigo", codigo);
            return Convert.ToInt64(comando.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Une por fecha las clases (con su estado) y los trabajos (con su
        /// título) de la comisión, para el calendario (H2, Escenario 3).
        /// </summary>
        public List<ElementoCalendario> VerCalendario(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT fecha, 'Clase', estado
                FROM clases c
                INNER JOIN comisiones com ON com.id = c.comision_id
                WHERE com.codigo = @codigo
                UNION ALL
                SELECT fecha_entrega, 'Trabajo', titulo
                FROM trabajos t
                INNER JOIN comisiones com ON com.id = t.comision_id
                WHERE com.codigo = @codigo
                ORDER BY 1;
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var elementos = new List<ElementoCalendario>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                elementos.Add(new ElementoCalendario
                {
                    Fecha = lector.GetDateTime(0),
                    Tipo = lector.GetString(1),
                    Detalle = lector.GetString(2),
                });
            return elementos;
        }
    }
}
