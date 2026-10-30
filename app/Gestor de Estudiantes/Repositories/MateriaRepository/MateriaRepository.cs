using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de materias y del vínculo N:M con comisiones sobre
    /// SQLite (tablas <c>materias</c> y <c>materia_comision</c>, QA001).
    /// Los nombres y códigos se comparan sin distinguir mayúsculas.
    /// </summary>
    public class MateriaRepository : IMateriaRepository
    {
        /// <summary>Devuelve todas las materias ordenadas por nombre.</summary>
        public List<Materia> ListarMaterias()
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT id, nombre FROM materias ORDER BY nombre;";

            var materias = new List<Materia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                materias.Add(Mapear(lector));
            return materias;
        }

        /// <summary>Inserta una materia nueva.</summary>
        public void AgregarMateria(Materia materia)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "INSERT INTO materias (id, nombre) VALUES (@id, @nombre);";
            comando.Parameters.AddWithValue("@id", materia.Id.ToString("D"));
            comando.Parameters.AddWithValue("@nombre", materia.Nombre);
            comando.ExecuteNonQuery();
        }

        /// <summary>Indica si existe una materia con ese nombre (ignorando caja).</summary>
        public bool ExisteMateria(string nombre)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText =
                "SELECT COUNT(*) FROM materias WHERE nombre COLLATE NOCASE = @nombre;";
            comando.Parameters.AddWithValue("@nombre", nombre);
            return Convert.ToInt64(comando.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Vincula materia y comisión resolviendo ambos por nombre/código;
        /// si el vínculo ya existe la PK compuesta lo ignora.
        /// </summary>
        public void AsignarMateria(string nombreMateria, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT OR IGNORE INTO materia_comision (materia_id, comision_id)
                VALUES (
                    (SELECT id FROM materias WHERE nombre COLLATE NOCASE = @nombre),
                    (SELECT id FROM comisiones WHERE codigo COLLATE NOCASE = @codigo));
                """;
            comando.Parameters.AddWithValue("@nombre", nombreMateria);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.ExecuteNonQuery();
        }

        /// <summary>Quita el vínculo materia-comisión.</summary>
        public bool DesasignarMateria(string nombreMateria, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                DELETE FROM materia_comision
                WHERE materia_id = (SELECT id FROM materias WHERE nombre COLLATE NOCASE = @nombre)
                  AND comision_id = (SELECT id FROM comisiones WHERE codigo COLLATE NOCASE = @codigo);
                """;
            comando.Parameters.AddWithValue("@nombre", nombreMateria);
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            return comando.ExecuteNonQuery() > 0;
        }

        /// <summary>Devuelve las materias de la comisión ordenadas por nombre.</summary>
        public List<Materia> ListarMateriasDeComision(string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT m.id, m.nombre
                FROM materias m
                INNER JOIN materia_comision mc ON mc.materia_id = m.id
                INNER JOIN comisiones com ON com.id = mc.comision_id
                WHERE com.codigo COLLATE NOCASE = @codigo
                ORDER BY m.nombre;
                """;
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            var materias = new List<Materia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                materias.Add(Mapear(lector));
            return materias;
        }

        /// <summary>Devuelve las comisiones de la materia ordenadas por código.</summary>
        public List<Comision> ListarComisionesDeMateria(string nombreMateria)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT com.id, com.codigo, com.semanas
                FROM comisiones com
                INNER JOIN materia_comision mc ON mc.comision_id = com.id
                INNER JOIN materias m ON m.id = mc.materia_id
                WHERE m.nombre COLLATE NOCASE = @nombre
                ORDER BY com.codigo;
                """;
            comando.Parameters.AddWithValue("@nombre", nombreMateria);

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

        private static Materia Mapear(SqliteDataReader lector) => new Materia
        {
            Id = Guid.Parse(lector.GetString(0)),
            Nombre = lector.GetString(1),
        };
    }
}
