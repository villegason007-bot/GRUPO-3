using Application.Servicios;
using Structure.Datos;
using Structure.Repositorios;

BaseDeDatos.Inicializar();

var gestor = new GestorDeEstudiantes(new RepositorioEstudiantes());
var comision = CargaDePrueba.Comision;

if (gestor.ListarEstudiantes(comision).Count == 0)
{
    var cargados = CargaDePrueba.Cargar(comision);
    Console.WriteLine($"Base inicializada: {cargados} estudiantes cargados en la comisión {comision}.");
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine($"=== Gestión de estudios ===  Comisión: {comision}");
    Console.WriteLine("1) Listar estudiantes");
    Console.WriteLine("2) Agregar estudiante");
    Console.WriteLine("3) Cargar 30 estudiantes de prueba");
    Console.WriteLine("0) Salir");
    Console.Write("Opción: ");

    var opcion = Console.ReadLine();
    if (opcion is null)
        return;

    switch (opcion)
    {
        case "1":
            Listar();
            break;
        case "2":
            Agregar();
            break;
        case "3":
            Console.WriteLine($"Se cargaron {CargaDePrueba.Cargar(comision)} estudiantes.");
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }
}

void Listar()
{
    var estudiantes = gestor.ListarEstudiantes(comision);

    Console.WriteLine();
    Console.WriteLine($"{"Legajo",-12}Nombre");
    Console.WriteLine(new string('-', 34));
    foreach (var estudiante in estudiantes)
        Console.WriteLine($"{estudiante.Legajo,-12}{estudiante.Nombre}");
    Console.WriteLine(new string('-', 34));
    Console.WriteLine($"Total: {estudiantes.Count}");
}

void Agregar()
{
    Console.Write("Legajo: ");
    var legajo = Console.ReadLine() ?? string.Empty;
    Console.Write("Nombre: ");
    var nombre = Console.ReadLine() ?? string.Empty;

    try
    {
        var estudiante = gestor.AgregarEstudiante(comision, legajo, nombre);
        Console.WriteLine($"Agregado: {estudiante}");
    }
    catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine($"Error: {excepcion.Message}");
    }
}
