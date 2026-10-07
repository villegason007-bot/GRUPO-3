using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Config
{
    public static class BaseDeDatos
    {
        public static string Ruta { get; } = ResolverRuta();

        public static string CadenaDeConexion
            => new SqliteConnectionStringBuilder
            {
                DataSource = Ruta,
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();

        public static SqliteConnection Abrir()
        {
            var conexion = new SqliteConnection(CadenaDeConexion);
            conexion.Open();
            return conexion;
        }

        public static void Inicializar()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);

            using var conexion = Abrir();

            using var esquema = conexion.CreateCommand();
            esquema.CommandText = """
                CREATE TABLE IF NOT EXISTS comisiones (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    codigo TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS estudiantes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    legajo TEXT NOT NULL UNIQUE,
                    nombre TEXT NOT NULL,
                    apellido TEXT NULL,
                    telefono TEXT NULL,
                    fecha_incorporacion TEXT NULL,
                    comision_id INTEGER NOT NULL REFERENCES comisiones(id)
                );
                """;
            esquema.ExecuteNonQuery();

            using var semilla = conexion.CreateCommand();
            semilla.CommandText = "INSERT OR IGNORE INTO comisiones (codigo) VALUES (@codigo);";
            semilla.Parameters.AddWithValue("@codigo", "1K1");
            semilla.ExecuteNonQuery();
        }

        private static string ResolverRuta()
        {
            var directorio = new DirectoryInfo(AppContext.BaseDirectory);

            while (directorio is not null)
            {
                if (Directory.Exists(Path.Combine(directorio.FullName, ".git")))
                    return Path.Combine(directorio.FullName, "app", "Gestor de Estudiantes", "data", "ppii.db");
                directorio = directorio.Parent;
            }

            return Path.Combine(AppContext.BaseDirectory, "data", "ppii.db");
        }
    }
}
