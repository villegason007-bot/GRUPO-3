using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de los conteos que alimentan el indicador de riesgo
    /// (H6) sobre SQLite: asistencias en clases hábiles y trabajos
    /// vencidos. Si el estudiante tiene fecha de incorporación (R2), solo
    /// se cuentan las clases y los trabajos desde esa fecha. Los parámetros
    /// configurables (H7) viven en <see cref="ParametrosRiesgoRepository"/>.
    /// </summary>
    public class RiesgoRepository : IRiesgoRepository
    {
        /// <summary>
        /// Fecha desde la que cursa el estudiante (R2), en formato aaaa-MM-dd.
        /// null si no tiene fecha cargada: en ese caso no se filtra nada.
        /// </summary>
        private static object FechaDesde(SqliteConnection conexion, Guid estudianteId)
        {
            using var comando = conexion.CreateCommand();
            comando.CommandText =
                "SELECT fecha_incorporacion FROM estudiantes WHERE id = @estudianteId;";
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));

            var valor = comando.ExecuteScalar();
            if (valor is null || valor is DBNull || string.IsNullOrWhiteSpace(valor.ToString()))
                return DBNull.Value;

            return valor.ToString()!;
        }

        /// <summary>
        /// Cuenta asistencia de un estudiante en una comisión considerando
        /// solo clases hábiles desde su fecha de incorporación (R2): total
        /// de clases hábiles, faltas del estudiante y si tiene alguna
        /// asistencia cargada (sin registros, el cálculo no promedia faltas).
        /// </summary>
        public (int ClasesHabiles, int Faltas, bool TieneRegistro) ContarAsistencias(
            Guid estudianteId, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();

            comando.CommandText = """
                SELECT
                    (
                        SELECT COUNT(*)
                        FROM clases c
                        INNER JOIN comisiones com ON com.id = c.comision_id
                        WHERE com.codigo = @codigo
                          AND c.estado = @estadoHabil
                          AND (@desde IS NULL OR date(c.fecha) >= date(@desde))
                    ),
                    COALESCE(
                        SUM(CASE
                            WHEN a.condicion = @condicionAusente THEN 1
                            ELSE 0
                        END),
                        0
                    ),
                    COUNT(CASE
                        WHEN a.condicion <> @condicionPendiente THEN 1
                    END)
                FROM asistencias a
                INNER JOIN clases c ON c.id = a.clase_id
                INNER JOIN comisiones com ON com.id = c.comision_id
                WHERE a.estudiante_id = @estudianteId
                  AND com.codigo = @codigo
                  AND c.estado = @estadoHabil
                  AND (@desde IS NULL OR date(c.fecha) >= date(@desde));
                """;

            comando.Parameters.AddWithValue(
                "@estadoHabil",
                EstadoClase.Habil.ToString());

            comando.Parameters.AddWithValue(
                "@condicionAusente",
                Condicion.Ausente.ToString());

            // Un registro aún sin marcar no cuenta como información:
            // el estudiante sigue en «Sin información» (H6, Escenario 2).
            comando.Parameters.AddWithValue(
                "@condicionPendiente",
                Condicion.Pendiente.ToString());

            comando.Parameters.AddWithValue(
                "@estudianteId",
                estudianteId.ToString("D"));

            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.Parameters.AddWithValue("@desde", FechaDesde(conexion, estudianteId));

            using var lector = comando.ExecuteReader();
            lector.Read();

            return (
                lector.GetInt32(0),
                lector.GetInt32(1),
                lector.GetInt32(2) > 0
            );
        }

        /// <summary>
        /// Devuelve las condiciones (Presente/Ausente) del estudiante en
        /// clases hábiles de la comisión desde su fecha de incorporación
        /// (R2), ordenadas de la más antigua a la más reciente. Las
        /// asistencias Pendiente no se incluyen (R1).
        /// </summary>
        public List<Condicion> ObtenerCondicionesPorFecha(
            Guid estudianteId, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();

            comando.CommandText = """
                SELECT a.condicion
                FROM asistencias a
                INNER JOIN clases c ON c.id = a.clase_id
                INNER JOIN comisiones com ON com.id = c.comision_id
                WHERE a.estudiante_id = @estudianteId
                  AND com.codigo = @codigo
                  AND c.estado = @estadoHabil
                  AND a.condicion <> @condicionPendiente
                  AND (@desde IS NULL OR date(c.fecha) >= date(@desde))
                ORDER BY c.fecha, c.id;
                """;

            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.Parameters.AddWithValue("@estadoHabil", EstadoClase.Habil.ToString());
            comando.Parameters.AddWithValue("@condicionPendiente", Condicion.Pendiente.ToString());
            comando.Parameters.AddWithValue("@desde", FechaDesde(conexion, estudianteId));

            var condiciones = new List<Condicion>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                condiciones.Add(Enum.Parse<Condicion>(lector.GetString(0)));

            return condiciones;
        }

        /// <summary>
        /// Cuenta los trabajos de la comisión ya vencidos a hoy, desde la
        /// fecha de incorporación del estudiante (R2), y de esos cuántos
        /// entregó.
        /// </summary>
        public (int TrabajosVencidos, int Entregados) ContarTrabajos(Guid estudianteId, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT COUNT(*),
                       COALESCE(SUM(CASE WHEN e.entregado = 1 THEN 1 ELSE 0 END), 0)
                FROM trabajos t
                LEFT JOIN entregas e ON e.trabajo_id = t.id AND e.estudiante_id = @estudianteId
                INNER JOIN comisiones com ON com.id = t.comision_id
                WHERE com.codigo = @codigo
                  AND date(t.fecha_entrega) <= date('now', 'localtime')
                  AND (@desde IS NULL OR date(t.fecha_entrega) >= date(@desde));
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", codigoComision);
            comando.Parameters.AddWithValue("@desde", FechaDesde(conexion, estudianteId));

            using var lector = comando.ExecuteReader();
            lector.Read();
            return (lector.GetInt32(0), lector.GetInt32(1));
        }
    }
}