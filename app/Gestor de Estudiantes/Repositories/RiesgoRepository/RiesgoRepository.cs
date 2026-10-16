using Gestor_de_Estudiantes.Config;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de los parámetros de riesgo y de los conteos que
    /// alimentan el indicador (H6/H7/H8), sobre SQLite.
    /// </summary>
    public class RiesgoRepository : IRiesgoRepository
    {
        private const string ClaveUmbral = "umbral_riesgo";

        /// <summary>Devuelve el umbral porcentual vigente (0-100) de <c>parametros_riesgo</c>.</summary>
        public double ObtenerUmbral()
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT valor FROM parametros_riesgo WHERE clave = @clave;";
            comando.Parameters.AddWithValue("@clave", ClaveUmbral);
            return Convert.ToDouble(comando.ExecuteScalar());
        }

        /// <summary>Guarda el umbral porcentual (0-100); lo usa la consola de desarrollo.</summary>
        /// <exception cref="ArgumentException">Si el umbral está fuera de 0-100.</exception>
        public void GuardarUmbral(double umbral)
        {
            if (umbral < 0 || umbral > 100)
                throw new ArgumentException("El umbral debe estar entre 0 y 100.");

            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                INSERT INTO parametros_riesgo (clave, valor) VALUES (@clave, @valor)
                ON CONFLICT(clave) DO UPDATE SET valor = excluded.valor;
                """;
            comando.Parameters.AddWithValue("@clave", ClaveUmbral);
            comando.Parameters.AddWithValue("@valor", umbral);
            comando.ExecuteNonQuery();
        }

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
                          AND c.estado = 'Habil'
                    ),
                    COALESCE(
                        SUM(CASE
                            WHEN a.condicion = 'Ausente' THEN 1
                            ELSE 0
                        END),
                        0
                    ),
                    COUNT(a.id)
                FROM asistencias a
                INNER JOIN clases c ON c.id = a.clase_id
                INNER JOIN comisiones com ON com.id = c.comision_id
                WHERE a.estudiante_id = @estudianteId
                  AND com.codigo = @codigo
                  AND c.estado = 'Habil';
                """;

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

        /// <summary>Devuelve el máximo de faltas permitidas; si no está guardado, el predeterminado es 3.</summary>
        public int ObtenerMaxFaltasPermitidas()
        {
            return ObtenerParametroEntero("max_faltas_permitidas", 3);
        }

        /// <summary>Guarda el máximo de faltas permitidas (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxFaltasPermitidas(int cantidad)
        {
            if (cantidad < 0)
                throw new ArgumentException(
                    "El máximo de faltas no puede ser negativo.");

            GuardarParametro("max_faltas_permitidas", cantidad);
        }

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos; si no está guardado, el predeterminado es 2.</summary>
        public int ObtenerMaxTrabajosNoEntregadosPermitidos()
        {
            return ObtenerParametroEntero(
                "max_trabajos_no_entregados_permitidos", 2);
        }

        /// <summary>Guarda el máximo de trabajos no entregados permitidos (H7).</summary>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        public void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad)
        {
            if (cantidad < 0)
                throw new ArgumentException(
                    "El máximo de trabajos no entregados no puede ser negativo.");

            GuardarParametro(
                "max_trabajos_no_entregados_permitidos",
                cantidad);
        }

        /// <summary>Lee un parámetro entero de <c>parametros_riesgo</c>; si no existe devuelve el valor predeterminado.</summary>
        private int ObtenerParametroEntero(string clave, int valorPredeterminado)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();

            comando.CommandText =
                "SELECT valor FROM parametros_riesgo WHERE clave = @clave;";
            comando.Parameters.AddWithValue("@clave", clave);

            var valor = comando.ExecuteScalar();

            if (valor == null || valor == DBNull.Value)
                return valorPredeterminado;

            return Convert.ToInt32(Convert.ToDouble(valor));
        }

        /// <summary>Guarda o reemplaza un parámetro entero en <c>parametros_riesgo</c> (upsert por clave).</summary>
        private void GuardarParametro(string clave, int valor)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();

            comando.CommandText = """
                INSERT INTO parametros_riesgo (clave, valor)
                VALUES (@clave, @valor)
                ON CONFLICT(clave)
                DO UPDATE SET valor = excluded.valor;
                """;

            comando.Parameters.AddWithValue("@clave", clave);
            comando.Parameters.AddWithValue("@valor", valor);

            comando.ExecuteNonQuery();
        }
    }
}
