using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia del calendario unificado de la comisión sobre SQLite:
    /// une por fecha las clases (con su estado) y los trabajos (con su
    /// título) de la comisión (H2, Escenario 3).
    /// </summary>
    public class CalendarioRepository : ICalendarioRepository
    {
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
