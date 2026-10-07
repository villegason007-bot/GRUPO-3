using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;
using Microsoft.Data.Sqlite;

const string ComisionActiva = "1K1";

BaseDeDatos.Inicializar();
Console.WriteLine($"Base de datos: {BaseDeDatos.Ruta}");
Console.WriteLine();

var servicioEstudiantes = new EstudianteService(new EstudianteRepository());
var servicioComisiones = new ComisionService(new ComisionRepository());
var repositorioClases = new ClaseRepository();
var repositorioTrabajos = new TrabajoRepository();
var repositorioAsistencias = new AsistenciaRepository();
var repositorioEntregas = new EntregaRepository();

while (true)
{
    Console.WriteLine("=== Gestor de Estudiantes ===");
    Console.WriteLine("1) Estudiantes");
    Console.WriteLine("2) Comisiones");
    Console.WriteLine("3) Clases");
    Console.WriteLine("4) Trabajos");
    Console.WriteLine("5) Asistencias");
    Console.WriteLine("6) Entregas");
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
        case "3":
            MenuClases();
            break;
        case "4":
            MenuTrabajos();
            break;
        case "5":
            MenuAsistencias();
            break;
        case "6":
            MenuEntregas();
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

void MenuClases()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine($"--- Clases · Comisión {ComisionActiva} ---");
        Console.WriteLine("1) Listar clases");
        Console.WriteLine("2) Agregar clase");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    var clases = repositorioClases.ListarClases(ComisionActiva);
                    Console.WriteLine();
                    Console.WriteLine($"{"Id",-6}Fecha");
                    Console.WriteLine(new string('-', 20));
                    foreach (var clase in clases)
                        Console.WriteLine($"{clase.Id,-6}{clase.Fecha:yyyy-MM-dd}");
                    Console.WriteLine($"Total: {clases.Count}");
                    break;
                case "2":
                    Console.Write("Fecha (aaaa-mm-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out var fechaClase))
                    {
                        Console.WriteLine("Fecha inválida.");
                        break;
                    }
                    repositorioClases.AgregarClase(new Clase { Fecha = fechaClase }, ComisionActiva);
                    Console.WriteLine($"Clase agregada: {fechaClase:yyyy-MM-dd}");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException or SqliteException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

void MenuTrabajos()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine($"--- Trabajos · Comisión {ComisionActiva} ---");
        Console.WriteLine("1) Listar trabajos");
        Console.WriteLine("2) Agregar trabajo");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    var trabajos = repositorioTrabajos.ListarTrabajos(ComisionActiva);
                    Console.WriteLine();
                    Console.WriteLine($"{"Id",-6}{"Entrega",-14}Título");
                    Console.WriteLine(new string('-', 44));
                    foreach (var trabajo in trabajos)
                        Console.WriteLine($"{trabajo.Id,-6}{trabajo.FechaDeEntrega,-14:yyyy-MM-dd}{trabajo.Titulo}");
                    Console.WriteLine($"Total: {trabajos.Count}");
                    break;
                case "2":
                    Console.Write("Título: ");
                    var titulo = Console.ReadLine() ?? string.Empty;
                    Console.Write("Fecha de entrega (aaaa-mm-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out var fechaEntrega))
                    {
                        Console.WriteLine("Fecha inválida.");
                        break;
                    }
                    repositorioTrabajos.AgregarTrabajo(
                        new Trabajo { Titulo = titulo, FechaDeEntrega = fechaEntrega },
                        ComisionActiva);
                    Console.WriteLine($"Trabajo agregado: {titulo} ({fechaEntrega:yyyy-MM-dd})");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException or SqliteException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

void MenuAsistencias()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("--- Asistencias ---");
        Console.WriteLine("1) Listar asistencias de una clase");
        Console.WriteLine("2) Registrar asistencia");
        Console.WriteLine("3) Corregir condición");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    var claseId = LeerId("Id de la clase: ");
                    if (claseId is null) break;
                    var asistencias = repositorioAsistencias.ListarPorClase(claseId.Value);
                    Console.WriteLine();
                    Console.WriteLine($"{"Id",-6}{"Estudiante",-14}Condición");
                    Console.WriteLine(new string('-', 34));
                    foreach (var asistencia in asistencias)
                        Console.WriteLine($"{asistencia.Id,-6}{asistencia.EstudianteId,-14}{asistencia.Condicion}");
                    Console.WriteLine($"Total: {asistencias.Count}");
                    break;
                case "2":
                    var claseRegistrar = LeerId("Id de la clase: ");
                    if (claseRegistrar is null) break;
                    var estudianteRegistrar = LeerId("Id del estudiante: ");
                    if (estudianteRegistrar is null) break;
                    var condicion = LeerCondicion();
                    if (condicion is null) break;
                    repositorioAsistencias.RegistrarAsistencia(new Asistencia
                    {
                        ClaseId = claseRegistrar.Value,
                        EstudianteId = estudianteRegistrar.Value,
                        Condicion = condicion.Value,
                    });
                    Console.WriteLine("Asistencia registrada.");
                    break;
                case "3":
                    var claseCorregir = LeerId("Id de la clase: ");
                    if (claseCorregir is null) break;
                    var estudianteCorregir = LeerId("Id del estudiante: ");
                    if (estudianteCorregir is null) break;
                    var condicionCorregida = LeerCondicion();
                    if (condicionCorregida is null) break;
                    var corregida = repositorioAsistencias.ActualizarCondicion(
                        claseCorregir.Value, estudianteCorregir.Value, condicionCorregida.Value);
                    Console.WriteLine(corregida
                        ? "Condición corregida."
                        : "No hay asistencia registrada para ese estudiante en esa clase.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException or SqliteException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

void MenuEntregas()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("--- Entregas ---");
        Console.WriteLine("1) Listar entregas de un trabajo");
        Console.WriteLine("2) Registrar entrega");
        Console.WriteLine("3) Corregir entrega");
        Console.WriteLine("0) Volver");
        Console.Write("> ");

        var opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                    var trabajoId = LeerId("Id del trabajo: ");
                    if (trabajoId is null) break;
                    var entregas = repositorioEntregas.ListarPorTrabajo(trabajoId.Value);
                    Console.WriteLine();
                    Console.WriteLine($"{"Id",-6}{"Estudiante",-14}{"Entregado",-12}Fecha");
                    Console.WriteLine(new string('-', 44));
                    foreach (var entrega in entregas)
                        Console.WriteLine($"{entrega.Id,-6}{entrega.EstudianteId,-14}{(entrega.Entregado ? "Sí" : "No"),-12}{entrega.Fecha:yyyy-MM-dd}");
                    Console.WriteLine($"Total: {entregas.Count}");
                    break;
                case "2":
                    var trabajoRegistrar = LeerId("Id del trabajo: ");
                    if (trabajoRegistrar is null) break;
                    var estudianteRegistrar = LeerId("Id del estudiante: ");
                    if (estudianteRegistrar is null) break;
                    var entregado = LeerSiNo("¿Entregado? (S/N): ");
                    if (entregado is null) break;
                    var fechaEntrega = LeerFecha("Fecha (aaaa-mm-dd, vacío si no aplica): ");
                    if (fechaEntrega == DateTime.MinValue) break;
                    repositorioEntregas.RegistrarEntrega(new Entrega
                    {
                        TrabajoId = trabajoRegistrar.Value,
                        EstudianteId = estudianteRegistrar.Value,
                        Entregado = entregado.Value,
                        Fecha = fechaEntrega,
                    });
                    Console.WriteLine("Entrega registrada.");
                    break;
                case "3":
                    var trabajoCorregir = LeerId("Id del trabajo: ");
                    if (trabajoCorregir is null) break;
                    var estudianteCorregir = LeerId("Id del estudiante: ");
                    if (estudianteCorregir is null) break;
                    var entregadoCorregido = LeerSiNo("¿Entregado? (S/N): ");
                    if (entregadoCorregido is null) break;
                    var fechaCorregida = LeerFecha("Fecha (aaaa-mm-dd, vacío si no aplica): ");
                    if (fechaCorregida == DateTime.MinValue) break;
                    var corregida = repositorioEntregas.ActualizarEntrega(
                        trabajoCorregir.Value, estudianteCorregir.Value,
                        entregadoCorregido.Value, fechaCorregida);
                    Console.WriteLine(corregida
                        ? "Entrega corregida."
                        : "No hay entrega registrada para ese estudiante en ese trabajo.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        catch (Exception excepcion) when (excepcion is ArgumentException or InvalidOperationException or SqliteException)
        {
            Console.WriteLine($"Error: {excepcion.Message}");
        }
    }
}

long? LeerId(string etiqueta)
{
    Console.Write(etiqueta);
    if (long.TryParse(Console.ReadLine(), out var id))
        return id;
    Console.WriteLine("Id inválido.");
    return null;
}

Condicion? LeerCondicion()
{
    Console.Write("Condición (P=presente / A=ausente): ");
    var texto = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
    if (texto == "P") return Condicion.Presente;
    if (texto == "A") return Condicion.Ausente;
    Console.WriteLine("Condición inválida.");
    return null;
}

bool? LeerSiNo(string etiqueta)
{
    Console.Write(etiqueta);
    var texto = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
    if (texto == "S") return true;
    if (texto == "N") return false;
    Console.WriteLine("Respuesta inválida.");
    return null;
}

DateTime? LeerFecha(string etiqueta)
{
    Console.Write(etiqueta);
    var texto = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(texto))
        return null;
    if (DateTime.TryParse(texto, out var fecha))
        return fecha;
    Console.WriteLine("Fecha inválida.");
    return DateTime.MinValue;
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
