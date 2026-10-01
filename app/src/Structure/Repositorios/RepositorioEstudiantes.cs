using Application.Interfaces;
using Domain.Models;
using Microsoft.Data.Sqlite;
using Structure.Datos;

namespace Structure.Repositorios;

public class RepositorioEstudiantes : IRepositorioEstudiantes
{
    public void Agregar(string nombreComision, Estudiante estudiante)
    {
        using var conexion = BaseDeDatos.Abrir();
        using var transaccion = conexion.BeginTransaction();

        var idComision = ObtenerOCrearComision(conexion, transaccion, nombreComision);

        using var comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText =
            """
            INSERT INTO estudiantes (legajo, nombre, id_comision)
            VALUES (@legajo, @nombre, @id_comision);
            """;
        comando.Parameters.AddWithValue("@legajo", estudiante.Legajo);
        comando.Parameters.AddWithValue("@nombre", estudiante.Nombre);
        comando.Parameters.AddWithValue("@id_comision", idComision);

        try
        {
            comando.ExecuteNonQuery();
        }
        catch (SqliteException excepcion) when (excepcion.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException(
                $"Ya existe un estudiante con legajo {estudiante.Legajo}.");
        }

        transaccion.Commit();
    }

    public IReadOnlyList<Estudiante> Listar(string nombreComision)
    {
        using var conexion = BaseDeDatos.Abrir();
        using var comando = conexion.CreateCommand();
        comando.CommandText =
            """
            SELECT e.legajo, e.nombre
            FROM estudiantes e
            INNER JOIN comisiones c ON c.id = e.id_comision
            WHERE c.nombre = @nombre
            ORDER BY e.legajo;
            """;
        comando.Parameters.AddWithValue("@nombre", nombreComision);

        var estudiantes = new List<Estudiante>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            estudiantes.Add(new Estudiante(
                lector.GetString(0),
                lector.GetString(1)));
        }

        return estudiantes;
    }

    public bool ExisteLegajo(string legajo)
    {
        using var conexion = BaseDeDatos.Abrir();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT 1 FROM estudiantes WHERE legajo = @legajo LIMIT 1;";
        comando.Parameters.AddWithValue("@legajo", legajo);

        return comando.ExecuteScalar() is not null;
    }

    private static long ObtenerOCrearComision(
        SqliteConnection conexion,
        SqliteTransaction transaccion,
        string nombreComision)
    {
        using var consulta = conexion.CreateCommand();
        consulta.Transaction = transaccion;
        consulta.CommandText = "SELECT id FROM comisiones WHERE nombre = @nombre;";
        consulta.Parameters.AddWithValue("@nombre", nombreComision);

        var existente = consulta.ExecuteScalar();
        if (existente is not null)
            return (long)existente;

        using var insercion = conexion.CreateCommand();
        insercion.Transaction = transaccion;
        insercion.CommandText = "INSERT INTO comisiones (nombre) VALUES (@nombre);";
        insercion.Parameters.AddWithValue("@nombre", nombreComision);
        insercion.ExecuteNonQuery();

        using var ultima = conexion.CreateCommand();
        ultima.Transaction = transaccion;
        ultima.CommandText = "SELECT last_insert_rowid();";
        return (long)ultima.ExecuteScalar()!;
    }
}
