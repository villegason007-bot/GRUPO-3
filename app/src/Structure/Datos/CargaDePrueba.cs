using Microsoft.Data.Sqlite;
using Structure.Datos;

namespace Structure.Datos;

public static class CargaDePrueba
{
    public const string Comision = "1K1";

    private static readonly (string Legajo, string Nombre)[] Estudiantes =
    {
        ("2026-001", "Valentina Ríos"),
        ("2026-002", "Martín Cabrera"),
        ("2026-003", "Lucía Ferreyra"),
        ("2026-004", "Juan Peralta"),
        ("2026-005", "Sofía Mansilla"),
        ("2026-006", "Diego Villalba"),
        ("2026-007", "Camila Ojeda"),
        ("2026-008", "Nicolás Bravo"),
        ("2026-009", "Julieta Ledesma"),
        ("2026-010", "Facundo Giménez"),
        ("2026-011", "Romina Sosa"),
        ("2026-012", "Agustín Moya"),
        ("2026-013", "Belén Acosta"),
        ("2026-014", "Sebastián Roldán"),
        ("2026-015", "Melina Quiroga"),
        ("2026-016", "Pablo Bustos"),
        ("2026-017", "Antonella Vera"),
        ("2026-018", "Gonzalo Ibarra"),
        ("2026-019", "Emilia Cáceres"),
        ("2026-020", "Rodrigo Núñez"),
        ("2026-021", "Jazmín Correa"),
        ("2026-022", "Franco Salinas"),
        ("2026-023", "Malena Aguirre"),
        ("2026-024", "Hernán Ponce"),
        ("2026-025", "Catalina Ruiz"),
        ("2026-026", "Ignacio Miranda"),
        ("2026-027", "Natalia Benítez"),
        ("2026-028", "Esteban Duarte"),
        ("2026-029", "Paula Espínola"),
        ("2026-030", "Leonardo Sandoval"),
    };

    public static int Cargar(string nombreComision = Comision)
    {
        using var conexion = BaseDeDatos.Abrir();
        using var transaccion = conexion.BeginTransaction();

        long idComision;
        using (var consulta = conexion.CreateCommand())
        {
            consulta.Transaction = transaccion;
            consulta.CommandText = "SELECT id FROM comisiones WHERE nombre = @nombre;";
            consulta.Parameters.AddWithValue("@nombre", nombreComision);

            var existente = consulta.ExecuteScalar();
            if (existente is not null)
            {
                idComision = (long)existente;
            }
            else
            {
                using var insercion = conexion.CreateCommand();
                insercion.Transaction = transaccion;
                insercion.CommandText = "INSERT INTO comisiones (nombre) VALUES (@nombre);";
                insercion.Parameters.AddWithValue("@nombre", nombreComision);
                insercion.ExecuteNonQuery();

                using var ultima = conexion.CreateCommand();
                ultima.Transaction = transaccion;
                ultima.CommandText = "SELECT last_insert_rowid();";
                idComision = (long)ultima.ExecuteScalar()!;
            }
        }

        var cargados = 0;
        foreach (var (legajo, nombre) in Estudiantes)
        {
            using var comando = conexion.CreateCommand();
            comando.Transaction = transaccion;
            comando.CommandText =
                """
                INSERT INTO estudiantes (legajo, nombre, id_comision)
                VALUES (@legajo, @nombre, @id_comision)
                ON CONFLICT (legajo) DO NOTHING;
                """;
            comando.Parameters.AddWithValue("@legajo", legajo);
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@id_comision", idComision);
            cargados += comando.ExecuteNonQuery();
        }

        transaccion.Commit();
        return cargados;
    }
}
