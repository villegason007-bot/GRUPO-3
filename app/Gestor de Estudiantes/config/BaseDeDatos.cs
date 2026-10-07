using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Config
{
    public static class BaseDeDatos
    {
        public static string Ruta { get; set; } = ResolverRuta();

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
                    id TEXT PRIMARY KEY,
                    codigo TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS estudiantes (
                    id TEXT PRIMARY KEY,
                    legajo TEXT NOT NULL UNIQUE,
                    nombre TEXT NOT NULL,
                    apellido TEXT NULL,
                    telefono TEXT NULL,
                    fecha_incorporacion TEXT NULL,
                    activo INTEGER NOT NULL DEFAULT 1 CHECK (activo IN (0, 1)),
                    comision_id TEXT NOT NULL REFERENCES comisiones(id)
                );

                CREATE TABLE IF NOT EXISTS clases (
                    id TEXT PRIMARY KEY,
                    comision_id TEXT NOT NULL REFERENCES comisiones(id),
                    fecha TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS asistencias (
                    id TEXT PRIMARY KEY,
                    clase_id TEXT NOT NULL REFERENCES clases(id),
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    condicion TEXT NOT NULL CHECK (condicion IN ('Presente', 'Ausente')),
                    UNIQUE (clase_id, estudiante_id)
                );

                CREATE TABLE IF NOT EXISTS trabajos (
                    id TEXT PRIMARY KEY,
                    comision_id TEXT NOT NULL REFERENCES comisiones(id),
                    titulo TEXT NOT NULL,
                    fecha_entrega TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS entregas (
                    id TEXT PRIMARY KEY,
                    trabajo_id TEXT NOT NULL REFERENCES trabajos(id),
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    entregado INTEGER NOT NULL CHECK (entregado IN (0, 1)),
                    fecha TEXT NULL,
                    UNIQUE (trabajo_id, estudiante_id)
                );
                """;
            esquema.ExecuteNonQuery();

            // Migración ligera para bases creadas antes de la baja lógica:
            // SQLite no admite ADD COLUMN IF NOT EXISTS, se ignora si ya existe.
            try
            {
                using var migracion = conexion.CreateCommand();
                migracion.CommandText = "ALTER TABLE estudiantes ADD COLUMN activo INTEGER NOT NULL DEFAULT 1;";
                migracion.ExecuteNonQuery();
            }
            catch (SqliteException)
            {
            }

            using var semilla = conexion.CreateCommand();
            semilla.CommandText = "INSERT OR IGNORE INTO comisiones (id, codigo) VALUES (@id, @codigo);";
            semilla.Parameters.AddWithValue("@id", Guid.NewGuid().ToString("D"));
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
