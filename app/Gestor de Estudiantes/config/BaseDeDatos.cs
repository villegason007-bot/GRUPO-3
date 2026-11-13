using System;
using System.Collections.Generic;
using System.IO;
using Gestor_de_Estudiantes.Entidades;
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
                    materia_id TEXT NULL REFERENCES materias(id),
                    fecha TEXT NOT NULL,
                    estado TEXT NOT NULL DEFAULT 'Habil' CHECK (estado IN ('Habil', 'NoHabil'))
                );

                CREATE TABLE IF NOT EXISTS asistencias (
                    id TEXT PRIMARY KEY,
                    clase_id TEXT NOT NULL REFERENCES clases(id),
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    condicion TEXT NOT NULL DEFAULT 'Pendiente'
                        CHECK (condicion IN ('Pendiente', 'Presente', 'Ausente')),
                    UNIQUE (clase_id, estudiante_id)
                );

                CREATE TABLE IF NOT EXISTS trabajos (
                    id TEXT PRIMARY KEY,
                    comision_id TEXT NOT NULL REFERENCES comisiones(id),
                    materia_id TEXT NULL REFERENCES materias(id),
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

                CREATE TABLE IF NOT EXISTS estudiante_materia (
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    materia_id TEXT NOT NULL REFERENCES materias(id),
                    PRIMARY KEY (estudiante_id, materia_id)
                );

                CREATE TABLE IF NOT EXISTS contactos (
                    id TEXT PRIMARY KEY,
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    fecha TEXT NOT NULL,
                    medio TEXT NOT NULL CHECK (medio IN ('Llamada', 'Mensaje')),
                    resultado TEXT NOT NULL CHECK (resultado IN ('Contesto', 'QuedoEnVeremos', 'SinRespuesta'))
                );
                """;
            esquema.ExecuteNonQuery();

            MigrarCheckDeAsistencias(conexion);

            var migraciones = new[]
            {
                "ALTER TABLE estudiantes ADD COLUMN activo INTEGER NOT NULL DEFAULT 1;",
                $"ALTER TABLE comisiones ADD COLUMN semanas INTEGER NOT NULL DEFAULT {Comision.SemanasPorDefecto};",
                "ALTER TABLE clases ADD COLUMN estado TEXT NOT NULL DEFAULT 'Habil';",
                "ALTER TABLE clases ADD COLUMN materia_id TEXT NULL;",
                "ALTER TABLE trabajos ADD COLUMN materia_id TEXT NULL;",
                "ALTER TABLE estudiantes ADD COLUMN dni TEXT NULL;",
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
                catch (SqliteException) { }
            }

            SeedComisionesEstudiantesYAsistencias(conexion);
        }

        private static void SeedComisionesEstudiantesYAsistencias(SqliteConnection conexion)
        {
            string[] codigosComisiones = { "Matemática T1", "Matemática T2", "Física 1", "Álgebra 1", "Cálculo 1" };

            using var cmdCheck = conexion.CreateCommand();
            cmdCheck.CommandText = "SELECT COUNT(*) FROM asistencias;";
            long existenAsistencias = (long)(cmdCheck.ExecuteScalar() ?? 0L);

            if (existenAsistencias > 0) return;

            var comisionesIds = new Dictionary<string, string>();
            var clasesPorComision = new Dictionary<string, List<string>>();

            using var transaction = conexion.BeginTransaction();

            using (var cmdLimpiar = conexion.CreateCommand())
            {
                cmdLimpiar.Transaction = transaction;
                cmdLimpiar.CommandText = """
                    DELETE FROM asistencias;
                    DELETE FROM entregas;
                    DELETE FROM estudiante_materia;
                    DELETE FROM contactos;
                    DELETE FROM estudiantes;
                    DELETE FROM materia_comision;
                    DELETE FROM clases;
                    DELETE FROM trabajos;
                    DELETE FROM comisiones;
                    """;
                cmdLimpiar.ExecuteNonQuery();
            }

            // 1. Creamos las 5 comisiones y 8 clases hábiles por cada comisión
            foreach (var codigo in codigosComisiones)
            {
                string comId = Guid.NewGuid().ToString("D");
                using var cmdInsCom = conexion.CreateCommand();
                cmdInsCom.Transaction = transaction;
                cmdInsCom.CommandText = "INSERT INTO comisiones (id, codigo, semanas) VALUES (@id, @codigo, @semanas);";
                cmdInsCom.Parameters.AddWithValue("@id", comId);
                cmdInsCom.Parameters.AddWithValue("@codigo", codigo);
                cmdInsCom.Parameters.AddWithValue("@semanas", Comision.SemanasPorDefecto);
                cmdInsCom.ExecuteNonQuery();

                comisionesIds[codigo] = comId;
                clasesPorComision[codigo] = new List<string>();

                // Creamos 8 fechas de clase para la comisión
                DateTime fechaBase = new DateTime(2026, 9, 14);
                for (int c = 0; c < 8; c++)
                {
                    string claseId = Guid.NewGuid().ToString("D");
                    string fechaStr = fechaBase.AddDays(c * 7).ToString("yyyy-MM-dd");

                    using var cmdClase = conexion.CreateCommand();
                    cmdClase.Transaction = transaction;
                    cmdClase.CommandText = """
                        INSERT INTO clases (id, comision_id, fecha, estado)
                        VALUES (@id, @comision_id, @fecha, 'Habil');
                        """;
                    cmdClase.Parameters.AddWithValue("@id", claseId);
                    cmdClase.Parameters.AddWithValue("@comision_id", comId);
                    cmdClase.Parameters.AddWithValue("@fecha", fechaStr);
                    cmdClase.ExecuteNonQuery();

                    clasesPorComision[codigo].Add(claseId);
                }
            }

            using var cmdRiesgo = conexion.CreateCommand();
            cmdRiesgo.Transaction = transaction;
            cmdRiesgo.CommandText = "INSERT OR IGNORE INTO parametros_riesgo (clave, valor) VALUES ('umbral_riesgo', 50);";
            cmdRiesgo.ExecuteNonQuery();

            string[] apellidos = { "Giménez", "Ferreyra", "Ocampo", "Ríos", "Benítez", "Pérez", "Gómez", "Rodríguez", "López", "Martínez", "González", "Sánchez", "Díaz", "Álvarez", "Romero", "Alonso", "Torres", "Ruiz", "Ramírez", "Flores" };
            string[] nombres = { "Facundo", "Lucía", "Martín", "Valentina", "Nicolás", "Elias", "Sofía", "Mateo", "Camila", "Joaquín", "Agustina", "Lucas", "Victoria", "Tomás", "Mariana" };

            Random rand = new Random(42);
            int dniBase = 38000000;
            int legajoBase = 40000;

            // 2. Creamos los 500 estudiantes y sus asistencias/inasistencias en esas 8 clases
            for (int i = 0; i < 500; i++)
            {
                string codigoComision = codigosComisiones[i % codigosComisiones.Length];
                string comisionId = comisionesIds[codigoComision];

                string apellido = apellidos[rand.Next(apellidos.Length)];
                string nombrePila = nombres[rand.Next(nombres.Length)];
                string nombreCompleto = $"{apellido} {nombrePila}";
                string legajo = (legajoBase + i + 1).ToString();
                string dni = (dniBase + i + 1).ToString();
                string estudianteId = Guid.NewGuid().ToString("D");

                using var cmdInsEst = conexion.CreateCommand();
                cmdInsEst.Transaction = transaction;
                cmdInsEst.CommandText = """
                    INSERT INTO estudiantes (id, legajo, nombre, apellido, dni, activo, comision_id)
                    VALUES (@id, @legajo, @nombre, @apellido, @dni, 1, @comision_id);
                    """;
                cmdInsEst.Parameters.AddWithValue("@id", estudianteId);
                cmdInsEst.Parameters.AddWithValue("@legajo", legajo);
                cmdInsEst.Parameters.AddWithValue("@nombre", nombreCompleto);
                cmdInsEst.Parameters.AddWithValue("@apellido", apellido);
                cmdInsEst.Parameters.AddWithValue("@dni", dni);
                cmdInsEst.Parameters.AddWithValue("@comision_id", comisionId);
                cmdInsEst.ExecuteNonQuery();

                var listaClases = clasesPorComision[codigoComision];
                foreach (var claseId in listaClases)
                {
                    // Probabilidad de falta variada (25% de probabilidad de faltar en cada clase)
                    string condicion = rand.Next(100) < 25 ? "Ausente" : "Presente";

                    using var cmdAsis = conexion.CreateCommand();
                    cmdAsis.Transaction = transaction;
                    cmdAsis.CommandText = """
                        INSERT INTO asistencias (id, clase_id, estudiante_id, condicion)
                        VALUES (@id, @clase_id, @estudiante_id, @condicion);
                        """;
                    cmdAsis.Parameters.AddWithValue("@id", Guid.NewGuid().ToString("D"));
                    cmdAsis.Parameters.AddWithValue("@clase_id", claseId);
                    cmdAsis.Parameters.AddWithValue("@estudiante_id", estudianteId);
                    cmdAsis.Parameters.AddWithValue("@condicion", condicion);
                    cmdAsis.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }

        private static void MigrarCheckDeAsistencias(SqliteConnection conexion)
        {
            using var consulta = conexion.CreateCommand();
            consulta.CommandText = "SELECT sql FROM sqlite_master WHERE type = 'table' AND name = 'asistencias';";
            var esquemaActual = consulta.ExecuteScalar() as string;

            if (esquemaActual is null || esquemaActual.Contains("Pendiente")) return;

            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                PRAGMA foreign_keys = OFF;
                BEGIN TRANSACTION;

                CREATE TABLE asistencias_migrada (
                    id TEXT PRIMARY KEY,
                    clase_id TEXT NOT NULL REFERENCES clases(id),
                    estudiante_id TEXT NOT NULL REFERENCES estudiantes(id),
                    condicion TEXT NOT NULL DEFAULT 'Pendiente'
                        CHECK (condicion IN ('Pendiente', 'Presente', 'Ausente')),
                    UNIQUE (clase_id, estudiante_id)
                );

                INSERT INTO asistencias_migrada (id, clase_id, estudiante_id, condicion)
                    SELECT id, clase_id, estudiante_id, condicion FROM asistencias;

                DROP TABLE asistencias;
                ALTER TABLE asistencias_migrada RENAME TO asistencias;

                COMMIT;
                PRAGMA foreign_keys = ON;
                """;
            comando.ExecuteNonQuery();
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