using Gestor_de_Estudiantes.Config;

namespace Gestor_de_Estudiantes.Repositories
{
    public class RiesgoRepository : IRiesgoRepository
    {
        private const string ClaveUmbral = "umbral_riesgo";

        public double ObtenerUmbral()
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT valor FROM parametros_riesgo WHERE clave = @clave;";
            comando.Parameters.AddWithValue("@clave", ClaveUmbral);
            return Convert.ToDouble(comando.ExecuteScalar());
        }

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

        public (int ClasesHabiles, int Faltas, bool TieneRegistro) ContarAsistencias(
            Guid estudianteId, string codigoComision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                SELECT (SELECT COUNT(*)
                          FROM clases c
                          INNER JOIN comisiones com ON com.id = c.comision_id
                         WHERE com.codigo = @codigo AND c.estado = 'Habil'),
                       COALESCE(SUM(CASE WHEN a.condicion = 'Ausente' THEN 1 ELSE 0 END), 0),
                       COUNT(a.id)
                FROM asistencias a
                INNER JOIN clases c ON c.id = a.clase_id AND c.estado = 'Habil'
                WHERE a.estudiante_id = @estudianteId;
                """;
            comando.Parameters.AddWithValue("@estudianteId", estudianteId.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", codigoComision);

            using var lector = comando.ExecuteReader();
            lector.Read();
            return (lector.GetInt32(0), lector.GetInt32(1), lector.GetInt32(2) > 0);
        }

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
