using Gestor_de_Estudiantes.Entidades;
using Microsoft.Data.Sqlite;

namespace Gestor_de_Estudiantes.Config
{
    /// <summary>
    /// Acceso a la base de datos SQLite embebida: ruta del archivo,
    /// cadena de conexión, apertura y creación del esquema.
    /// </summary>
    public static class BaseDeDatos
    {
        /// <summary>
        /// Ruta del archivo <c>ppii.db</c>. Se resuelve una vez subiendo
        /// desde el binario hasta encontrar la raíz del repositorio (carpeta
        /// con <c>.git</c>); si no la encuentra, usa <c>data/</c> junto al ejecutable.
        /// </summary>
        public static string Ruta { get; set; } = ResolverRuta();

        /// <summary>Cadena de conexión Microsoft.Data.Sqlite hacia <see cref="Ruta"/> (modo ReadWriteCreate).</summary>
        public static string CadenaDeConexion
            => new SqliteConnectionStringBuilder
            {
                DataSource = Ruta,
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();

        /// <summary>
        /// Crea y abre una conexión nueva. Cada operación la usa con
        /// <c>using</c> y la cierra al terminar; no se comparte una
        /// conexión única entre operaciones.
        /// </summary>
        public static SqliteConnection Abrir()
        {
            var conexion = new SqliteConnection(CadenaDeConexion);
            conexion.Open();
            return conexion;
        }

        /// <summary>
        /// Inicializa la base: crea el directorio si hace falta, ejecuta el
        /// esquema (<c>CREATE TABLE IF NOT EXISTS</c>), aplica las
        /// migraciones ligeras de columnas nuevas y siembra la comisión
        /// inicial 1K1 con el umbral de riesgo por defecto.
        /// </summary>
        public static void Inicializar()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);

            using var conexion = Abrir();

            using var esquema = conexion.CreateCommand();
            esquema.CommandText = """
                CREATE TABLE IF NOT EXISTS comisiones (
                    id TEXT PRIMARY KEY,
                    codigo TEXT NOT NULL UNIQUE,
                    semanas INTEGER NOT NULL DEFAULT 16 CHECK (semanas > 0)
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
                    fecha TEXT NOT NULL,
                    estado TEXT NOT NULL DEFAULT 'Habil' CHECK (estado IN ('Habil', 'NoHabil'))
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

                CREATE TABLE IF NOT EXISTS parametros_riesgo (
                    clave TEXT PRIMARY KEY,
                    valor REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS materias (
                    id TEXT PRIMARY KEY,
                    nombre TEXT NOT NULL COLLATE NOCASE UNIQUE
                );

                CREATE TABLE IF NOT EXISTS materia_comision (
                    materia_id TEXT NOT NULL REFERENCES materias(id),
                    comision_id TEXT NOT NULL REFERENCES comisiones(id),
                    PRIMARY KEY (materia_id, comision_id)
                );
                """;
            esquema.ExecuteNonQuery();

            // Migraciones ligeras para bases creadas antes de las columnas nuevas:
            // SQLite no admite ADD COLUMN IF NOT EXISTS, se ignora si ya existe.
            var migraciones = new[]
            {
                "ALTER TABLE estudiantes ADD COLUMN activo INTEGER NOT NULL DEFAULT 1;",
                $"ALTER TABLE comisiones ADD COLUMN semanas INTEGER NOT NULL DEFAULT {Comision.SemanasPorDefecto};",
                "ALTER TABLE clases ADD COLUMN estado TEXT NOT NULL DEFAULT 'Habil';",
                // QA002: '1k1' no debe chocar con '1K1'. El índice único
                // sin distinguir caja se agrega después del esquema porque
                // en bases viejas la columna UNIQUE era case-sensitive.
                "CREATE UNIQUE INDEX IF NOT EXISTS ux_comisiones_codigo_nocase ON comisiones(codigo COLLATE NOCASE);",
            };

            foreach (var migracion in migraciones)
            {
                try
                {
                    using var comando = conexion.CreateCommand();
                    comando.CommandText = migracion;
                    comando.ExecuteNonQuery();
                }
                catch (SqliteException)
                {
                }
            }

            using var semilla = conexion.CreateCommand();
            semilla.CommandText = """
                INSERT OR IGNORE INTO comisiones (id, codigo, semanas) VALUES (@id, @codigo, @semanas);
                INSERT OR IGNORE INTO parametros_riesgo (clave, valor) VALUES ('umbral_riesgo', 50);
                """;
            semilla.Parameters.AddWithValue("@id", Guid.NewGuid().ToString("D"));
            semilla.Parameters.AddWithValue("@codigo", "1K1");
            semilla.Parameters.AddWithValue("@semanas", Comision.SemanasPorDefecto);
            semilla.ExecuteNonQuery();
        }

        /// <summary>Busca la raíz del repositorio subiendo desde <c>AppContext.BaseDirectory</c> para ubicar <c>data/ppii.db</c>.</summary>
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
