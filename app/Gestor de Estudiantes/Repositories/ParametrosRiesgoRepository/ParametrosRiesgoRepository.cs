using Gestor_de_Estudiantes.Config;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Persistencia de los parámetros configurables del indicador de
    /// riesgo (H7) sobre <c>parametros_riesgo</c> (tabla clave/valor),
    /// con validación de rangos. SRP: los conteos de riesgo (H6) viven
    /// en <see cref="RiesgoRepository"/>.
    /// </summary>
    public class ParametrosRiesgoRepository : IParametrosRiesgoRepository
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

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
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
