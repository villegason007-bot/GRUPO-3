using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de los contactos del tutor sobre SQLite (H10/H11).
    /// Como la fecha es a nivel de día, el «último» se resuelve por orden
    /// de inserción (<c>rowid</c>) dentro de la fecha más alta.
    /// </summary>
    public class ContactoRepository : IContactoRepository
    {
        /// <summary>Inserta un contacto nuevo.</summary>
        public void RegistrarContacto(Contacto contacto)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO contactos (id, estudiante_id, fecha, medio, resultado)
                VALUES (@id, @estudianteId, @fecha, @medio, @resultado);
                """;
            comando.Parameters.AddWithValue("@id", contacto.Id.ToString("D"));
            comando.Parameters.AddWithValue("@estudianteId", contacto.EstudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@fecha", contacto.Fecha);
            comando.Parameters.AddWithValue("@medio", contacto.Medio.ToString());
            comando.Parameters.AddWithValue("@resultado", contacto.Resultado.ToString());
            comando.ExecuteNonQuery();
        }

        /// <summary>Devuelve los contactos de un estudiante, del más reciente al más antiguo.</summary>
        public List<Contacto> ListarContactos(Guid estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, estudiante_id, fecha, medio, resultado
                FROM contactos
                WHERE estudiante_id = @estudianteId
                ORDER BY fecha DESC, rowid DESC;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            var contactos = new List<Contacto>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                contactos.Add(Mapear(lector));
            return contactos;
        }

        /// <summary>Devuelve el contacto más reciente de un estudiante, o null.</summary>
        public Contacto? UltimoContacto(Guid estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, estudiante_id, fecha, medio, resultado
                FROM contactos
                WHERE estudiante_id = @estudianteId
                ORDER BY fecha DESC, rowid DESC
                LIMIT 1;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            using var lector = comando.ExecuteReader();
            return lector.Read() ? Mapear(lector) : null;
        }

        /// <summary>Devuelve el último contacto de cada estudiante contactado de la comisión.</summary>
        public Dictionary<Guid, Contacto> UltimoContactoPorEstudiante(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT c.id, c.estudiante_id, c.fecha, c.medio, c.resultado
                FROM contactos c
                INNER JOIN estudiantes e ON e.id = c.estudiante_id
                INNER JOIN comisiones com ON com.id = e.comision_id
                WHERE com.codigo COLLATE NOCASE = @codigo
                  AND c.rowid = (
                      SELECT MAX(c2.rowid)
                      FROM contactos c2
                      WHERE c2.estudiante_id = c.estudiante_id);
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var ultimos = new Dictionary<Guid, Contacto>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                var contacto = Mapear(lector);
                ultimos[contacto.EstudianteId] = contacto;
            }
            return ultimos;
        }

        private static Contacto Mapear(SqliteDataReader lector) => new Contacto
        {
            Id = Guid.Parse(lector.GetString(0)),
            EstudianteId = Guid.Parse(lector.GetString(1)),
            Fecha = lector.GetDateTime(2),
            Medio = Enum.Parse<MedioContacto>(lector.GetString(3)),
            Resultado = Enum.Parse<ResultadoContacto>(lector.GetString(4)),
        };
    }
}
