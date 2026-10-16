using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de asistencias sobre SQLite (H3/H4).
    /// </summary>
    public class AsistenciaRepository : IAsistenciaRepository
    {
        /// <summary>
        /// Inserta una asistencia; se niega si la clase no es hábil
        /// (feriado o suspensión no reciben asistencia).
        /// </summary>
        /// <param name="asistencia">Registro a crear.</param>
        /// <exception cref="InvalidOperationException">Si la clase es no hábil.</exception>
        public void RegistrarAsistencia(Asistencia asistencia)
        {
            using var conexion = BaseDeDatos.Abrir();

            if (ClaseRepository.EstadoDe(conexion, asistencia.ClaseId) == EstadoClase.NoHabil)
                throw new InvalidOperationException(
                    "La clase no es hábil (feriado o suspensión): no se registra asistencia.");

            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO asistencias (id, clase_id, estudiante_id, condicion)
                VALUES (@id, @claseId, @estudianteId, @condicion);
                """;
            comando.Parameters.AddWithValue("@id", asistencia.Id.ToString("D"));
            comando.Parameters.AddWithValue("@claseId", asistencia.ClaseId.ToString("D"));
            comando.Parameters.AddWithValue("@estudianteId", asistencia.EstudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@condicion", asistencia.Condicion.ToString());
            comando.ExecuteNonQuery();
        }

        /// <summary>Cambia la condición (presente/ausente) de un registro existente.</summary>
        /// <returns>true si había registro y se actualizó.</returns>
        public bool ActualizarCondicion(Guid claseId, Guid estudianteId, Condicion condicion)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                UPDATE asistencias
                SET condicion = @condicion
                WHERE clase_id = @claseId AND estudiante_id = @estudianteId;
                """;
            comando.Parameters.AddWithValue("@condicion", condicion.ToString());
            comando.Parameters.AddWithValue("@claseId", claseId.ToString("D"));
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            return comando.ExecuteNonQuery() > 0;
        }

        /// <summary>Devuelve las asistencias de una clase, ordenadas por estudiante.</summary>
        public List<Asistencia> ListarPorClase(Guid claseId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, clase_id, estudiante_id, condicion
                FROM asistencias
                WHERE clase_id = @claseId
                ORDER BY estudiante_id;
                """;
            comando.Parameters.AddWithValue("@claseId", claseId.ToString("D"));

            var asistencias = new List<Asistencia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                asistencias.Add(Mapear(lector));
            return asistencias;
        }

        /// <summary>Devuelve las asistencias de un estudiante, ordenadas por clase.</summary>
        public List<Asistencia> ListarPorEstudiante(Guid estudianteId)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT id, clase_id, estudiante_id, condicion
                FROM asistencias
                WHERE estudiante_id = @estudianteId
                ORDER BY clase_id;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            var asistencias = new List<Asistencia>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                asistencias.Add(Mapear(lector));
            return asistencias;
        }

        private static Asistencia Mapear(SqliteDataReader lector) => new Asistencia
        {
            Id = Guid.Parse(lector.GetString(0)),
            ClaseId = Guid.Parse(lector.GetString(1)),
            EstudianteId = Guid.Parse(lector.GetString(2)),
            Condicion = Enum.Parse<Condicion>(lector.GetString(3)),
        };
    }
}
