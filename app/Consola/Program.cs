using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

const string ComisionActiva = "1K1";

BaseDeDatos.Inicializar();
Console.WriteLine($"Base de datos: {BaseDeDatos.Ruta}");
Console.WriteLine();

var servicioEstudiantes = new EstudianteService(new EstudianteRepository());
var servicioComisiones = new ComisionService(new ComisionRepository());

while (true)
{
    Console.WriteLine("=== Gestor de Estudiantes ===");
    Console.WriteLine("1) Estudiantes");
    Console.WriteLine("2) Comisiones");
    Console.WriteLine("0) Salir");
    Console.Write("> ");

    switch (Console.ReadLine())
    {
        case "1":
            MenuEstudiantes();
            break;
        case "2":
            MenuComisiones();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }

    Console.WriteLine();
}

void MenuEstudiantes()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine($"--- Estudiantes · Comisión {ComisionActiva} ---");
        Console.WriteLine("1) Listar estudiantes");
        Console.WriteLine("2) Agregar estudiante");
        Console.WriteLine("3) Buscar estudiante (legajo)");
        Console.WriteLine("4) Eliminar estudiante (legajo)");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    Listar();
                    break;
                case "2":
                    Agregar();
                    break;
                case "3":
                    Buscar();
                    break;
                case "4":
                    Eliminar();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

void MenuComisiones()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("--- Comisiones ---");
        Console.WriteLine("1) Listar comisiones");
        Console.WriteLine("2) Agregar comisión");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    var comisiones = servicioComisiones.ListarComisiones();
                    Console.WriteLine();
                    foreach (var comision in comisiones)
                        Console.WriteLine(comision.Codigo);
                    Console.WriteLine($"Total: {comisiones.Count}");
                    break;
                case "2":
                    Console.Write("Código: ");
                    var codigo = Console.ReadLine() ?? string.Empty;
                    var agregada = servicioComisiones.AgregarComision(codigo);
                    Console.WriteLine($"Agregada: {agregada.Codigo}");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

void Listar()
{
    var estudiantes = servicioEstudiantes.ListarEstudiantes(ComisionActiva);

    Console.WriteLine();
    Console.WriteLine($"{"Legajo",-12}Nombre");
    Console.WriteLine(new string('-', 34));
    if (estudiantes.Count == 0)
        Console.WriteLine("(sin estudiantes — cargá con la opción 2)");
    foreach (var estudiante in estudiantes)
        Console.WriteLine($"{estudiante.Legajo,-12}{estudiante.Nombre} {estudiante.Apellido}".TrimEnd());
    Console.WriteLine(new string('-', 34));
    Console.WriteLine($"Total: {estudiantes.Count}");
}

void Agregar()
{
    Console.Write("Legajo: ");
    var legajo = Console.ReadLine() ?? string.Empty;
    Console.Write("Nombre: ");
    var nombre = Console.ReadLine() ?? string.Empty;

    var estudiante = servicioEstudiantes.AgregarEstudiante(legajo, nombre, ComisionActiva);
    Console.WriteLine($"Agregado: {estudiante.Legajo} {estudiante.Nombre}");
}

void Buscar()
{
    Console.Write("Legajo: ");
    var legajo = Console.ReadLine() ?? string.Empty;

    var estudiante = servicioEstudiantes.BuscarEstudiante(legajo);
    if (estudiante is null)
        Console.WriteLine($"No existe estudiante con legajo {legajo.Trim()}.");
    else
        Console.WriteLine($"{estudiante.Legajo} · {estudiante.Nombre} {estudiante.Apellido}".TrimEnd('·', ' '));
}

void Eliminar()
{
    Console.Write("Legajo: ");
    var legajo = Console.ReadLine() ?? string.Empty;

    if (servicioEstudiantes.EliminarEstudiante(legajo))
        Console.WriteLine($"Eliminado: {legajo.Trim()}");
    else
        Console.WriteLine($"No existe estudiante con legajo {legajo.Trim()}.");
}
