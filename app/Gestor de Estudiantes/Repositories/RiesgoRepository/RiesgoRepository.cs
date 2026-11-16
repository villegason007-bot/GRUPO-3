using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de los conteos que alimentan el indicador de riesgo
    /// (H6) sobre SQLite: asistencias en clases hábiles y trabajos
    /// vencidos. Los parámetros configurables (H7) viven en
    /// <see cref="ParametrosRiesgoRepository"/>.
    /// </summary>
    public class RiesgoRepository : IRiesgoRepository
    {
        /// <summary>
        /// Cuenta asistencia de un estudiante en una comisión considerando
        /// solo clases hábiles: total de clases hábiles de la comisión,
        /// faltas del estudiante y si tiene alguna asistencia cargada
        /// (sin registros, el cálculo no promedia faltas).
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
                  AND c.estado = @estadoHabil;
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

            using var lector = comando.ExecuteReader();
            lector.Read();

            return (
                lector.GetInt32(0),
                lector.GetInt32(1),
                lector.GetInt32(2) > 0
            );
        }

        /// <summary>
        /// Cuenta los trabajos de la comisión ya vencidos a hoy y, de esos,
        /// cuántos entregó el estudiante.
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
                  AND date(t.fecha_entrega) <= date('now', 'localtime');
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            using var lector = comando.ExecuteReader();
            lector.Read();
            return (lector.GetInt32(0), lector.GetInt32(1));
        }
    }
}
