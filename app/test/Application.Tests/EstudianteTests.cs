using Domain.Models;
using Xunit;

namespace Domain.Tests;

public class EstudianteTests
{
    [Fact]
    public void Constructor_NombreVacio_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Estudiante("2026-001", "  "));
    }

    [Fact]
    public void Constructor_DatosValidos_RecortaLosEspacios()
    {
        var estudiante = new Estudiante(" 2026-001 ", " Ana Pérez ");

        Assert.Equal("2026-001", estudiante.Legajo);
        Assert.Equal("Ana Pérez", estudiante.Nombre);
    }
}
