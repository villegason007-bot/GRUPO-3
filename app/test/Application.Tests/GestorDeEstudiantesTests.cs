using Application.Servicios;
using Xunit;

namespace Application.Tests;

public class GestorDeEstudiantesTests
{
    private const string Comision = "1K1";

    private static GestorDeEstudiantes CrearGestor()
        => new(new RepositorioEstudiantesEnMemoria());

    [Fact]
    public void AgregarEstudiante_LoGuarda_Y_LoDevuelve_ListarEstudiantes()
    {
        var gestor = CrearGestor();

        var estudiante = gestor.AgregarEstudiante(Comision, "2026-001", "Ana Pérez");

        Assert.Equal("2026-001", estudiante.Legajo);
        Assert.Equal("Ana Pérez", estudiante.Nombre);

        var nomina = gestor.ListarEstudiantes(Comision);
        var unico = Assert.Single(nomina);
        Assert.Equal("2026-001", unico.Legajo);
    }

    [Fact]
    public void AgregarEstudiante_LegajoDuplicado_LanzaInvalidOperationException()
    {
        var gestor = CrearGestor();
        gestor.AgregarEstudiante(Comision, "2026-001", "Ana Pérez");

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => gestor.AgregarEstudiante(Comision, "2026-001", "Otra Persona"));

        Assert.Contains("2026-001", excepcion.Message);
        Assert.Single(gestor.ListarEstudiantes(Comision));
    }

    [Fact]
    public void AgregarEstudiante_LegajoVacio_LanzaArgumentException()
    {
        var gestor = CrearGestor();

        Assert.Throws<ArgumentException>(
            () => gestor.AgregarEstudiante(Comision, "   ", "Ana Pérez"));

        Assert.Empty(gestor.ListarEstudiantes(Comision));
    }

    [Fact]
    public void ListarEstudiantes_ComisionVacia_DevuelveListaVacia()
    {
        var gestor = CrearGestor();

        Assert.Empty(gestor.ListarEstudiantes(Comision));
    }
}
