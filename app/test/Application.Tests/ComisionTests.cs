using Domain.Models;
using Xunit;

namespace Domain.Tests;

public class ComisionTests
{
    [Fact]
    public void Inscribir_LegajoRepetido_LanzaInvalidOperationException()
    {
        var comision = new Comision("1K1");
        comision.Inscribir(new Estudiante("2026-001", "Ana Pérez"));

        Assert.Throws<InvalidOperationException>(
            () => comision.Inscribir(new Estudiante("2026-001", "Otra Persona")));

        Assert.Single(comision.Estudiantes);
    }

    [Fact]
    public void Inscribir_EstudiantesDistintos_LosAgregaALaNomina()
    {
        var comision = new Comision("1K1");

        comision.Inscribir(new Estudiante("2026-001", "Ana Pérez"));
        comision.Inscribir(new Estudiante("2026-002", "Luis Gómez"));

        Assert.Equal(2, comision.Estudiantes.Count);
    }
}
