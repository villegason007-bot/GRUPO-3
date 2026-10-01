using Microsoft.Data.Sqlite;

namespace Structure.Datos;

public static class BaseDeDatos
{
    private const int VersionDeEsquema = 1;

    private static readonly string Ruta = ResolverRuta();

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

        using var comando = conexion.CreateCommand();
        comando.CommandText = "PRAGMA foreign_keys = ON;";
        comando.ExecuteNonQuery();

        return conexion;
    }

    public static void Inicializar()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);

        using var conexion = Abrir();

        using var modo = conexion.CreateCommand();
        modo.CommandText = "PRAGMA journal_mode = WAL;";
        modo.ExecuteNonQuery();

        using var version = conexion.CreateCommand();
        version.CommandText = "PRAGMA user_version;";
        var actual = Convert.ToInt32(version.ExecuteScalar());

        if (actual >= VersionDeEsquema)
            return;

        using var transaccion = conexion.BeginTransaction();
        using var comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText =
            """
            CREATE TABLE IF NOT EXISTS comisiones (
                id     INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS estudiantes (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                legajo      TEXT NOT NULL UNIQUE,
                nombre      TEXT NOT NULL,
                id_comision INTEGER NOT NULL,
                FOREIGN KEY (id_comision) REFERENCES comisiones (id)
            );

            CREATE INDEX IF NOT EXISTS idx_estudiantes_comision ON estudiantes (id_comision);

            PRAGMA user_version = 1;
            """;
        comando.ExecuteNonQuery();
        transaccion.Commit();
    }

    private static string ResolverRuta()
    {
        var directorio = new DirectoryInfo(AppContext.BaseDirectory);

        while (directorio is not null && !File.Exists(Path.Combine(directorio.FullName, "app.sln")))
            directorio = directorio.Parent;

        var raiz = directorio?.FullName ?? Directory.GetCurrentDirectory();
        return Path.Combine(raiz, "data", "ppii.db");
    }
}
