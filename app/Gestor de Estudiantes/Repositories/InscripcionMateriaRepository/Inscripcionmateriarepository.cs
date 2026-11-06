using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Inscripciones de estudiantes a materias sobre SQLite (tabla
    /// <c>estudiante_materia</c>). Solo se puede inscribir en materias
    /// asignadas a la comisión del estudiante.
    /// </summary>
    public class InscripcionMateriaRepository : IInscripcionMateriaRepository
    {
        public List<Materia> ListarMateriasDeEstudiante(Guid estudianteId)
        {
            var materias = new List<Materia>();

            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT m.id, m.nombre
                FROM estudiante_materia em
                INNER JOIN materias m ON m.id = em.materia_id
                INNER JOIN estudiantes e ON e.id = em.estudiante_id
                WHERE em.estudiante_id = @estudianteId
                  AND e.activo = 1
                ORDER BY m.nombre COLLATE NOCASE;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            using var reader = comando.ExecuteReader();
            while (reader.Read())
            {
                materias.Add(new Materia
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Nombre = reader.GetString(1),
                });
            }

            return materias;
        }

        public List<Estudiante> ListarEstudiantesDeMateria(
            Guid materiaId,
            string? codigoComision = null)
        {
            var estudiantes = new List<Estudiante>();

            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT e.id, e.legajo, e.nombre, e.apellido, e.telefono,
                       e.fecha_incorporacion, c.codigo
                FROM estudiante_materia em
                INNER JOIN estudiantes e ON e.id = em.estudiante_id
                INNER JOIN comisiones c ON c.id = e.comision_id
                WHERE em.materia_id = @materiaId
                  AND e.activo = 1
                  AND (@codigoComision IS NULL
                       OR c.codigo = @codigoComision COLLATE NOCASE)
                ORDER BY e.apellido COLLATE NOCASE, e.nombre COLLATE NOCASE;
                """;
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));
            comando.Parameters.AddWithValue(
                "@codigoComision",
                string.IsNullOrWhiteSpace(codigoComision)
                    ? DBNull.Value
                    : codigoComision.Trim());

            using var reader = comando.ExecuteReader();
            while (reader.Read())
            {
                estudiantes.Add(new Estudiante
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Legajo = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Apellido = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                    FechaIncorporacion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CodigoComision = reader.GetString(6),
                });
            }

            return estudiantes;
        }

        public bool PuedeInscribir(Guid estudianteId, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM estudiantes e
                    INNER JOIN materia_comision mc ON mc.comision_id = e.comision_id
                    WHERE e.id = @estudianteId
                      AND mc.materia_id = @materiaId
                      AND e.activo = 1
                );
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));

            return Convert.ToInt32(comando.ExecuteScalar()) == 1;
        }

        public bool EstaInscripto(Guid estudianteId, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM estudiante_materia
                    WHERE estudiante_id = @estudianteId
                      AND materia_id = @materiaId
                );
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));

            return Convert.ToInt32(comando.ExecuteScalar()) == 1;
        }

        public bool Inscribir(Guid estudianteId, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT OR IGNORE INTO estudiante_materia (estudiante_id, materia_id)
                SELECT e.id, mc.materia_id
                FROM estudiantes e
                INNER JOIN materia_comision mc ON mc.comision_id = e.comision_id
                WHERE e.id = @estudianteId
                  AND mc.materia_id = @materiaId
                  AND e.activo = 1;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));

            return comando.ExecuteNonQuery() > 0;
        }

        public bool Desinscribir(Guid estudianteId, Guid materiaId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                DELETE FROM estudiante_materia
                WHERE estudiante_id = @estudianteId
                  AND materia_id = @materiaId;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@materiaId", materiaId.ToString("D"));

            return comando.ExecuteNonQuery() > 0;
        }
    }
}