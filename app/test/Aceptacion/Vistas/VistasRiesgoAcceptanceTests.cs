using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test.Aceptacion.Vistas;

[TestClass]
[DoNotParallelize]
public sealed class VistasRiesgoAcceptanceTests : RepositorioTestBase
{
    private static RiesgoService CrearRiesgo() => new(
        new RiesgoRepository(), new ParametrosRiesgoRepository(),
        new EstudianteRepository(), new CalculadoraDeRiesgo(),
        new ContactoRepository());

    [TestMethod]
    public void ConfiguracionRiesgoPage_GuardarParametros_LosMuestraAlVolverACargar()
    {
        var servicio = new ParametrosRiesgoService(new ParametrosRiesgoRepository());

        servicio.GuardarUmbral(70);
        servicio.GuardarMaxFaltasPermitidas(4);
        servicio.GuardarMaxTrabajosNoEntregadosPermitidos(3);

        Assert.AreEqual(70, servicio.ObtenerUmbral());
        Assert.AreEqual(4, servicio.ObtenerMaxFaltasPermitidas());
        Assert.AreEqual(3, servicio.ObtenerMaxTrabajosNoEntregadosPermitidos());
    }

    [TestMethod]
    public void RiesgoPage_EstudianteConFalta_ExponeEstadoPuntajeYMotivo()
    {
        var estudianteId = CrearEstudiante("VR100", "Rocío");
        var claseId = CrearClase(DateTime.Today.AddDays(-7));
        new AsistenciaService(new AsistenciaRepository())
            .GuardarAsistencia(claseId, estudianteId, Condicion.Ausente);

        var fila = CrearRiesgo().CalcularRiesgos("1K1").Single();

        Assert.AreEqual("VR100", fila.Legajo);
        Assert.AreEqual(100, fila.Puntaje);
        Assert.AreEqual("En riesgo", fila.Estado);
        Assert.IsFalse(string.IsNullOrWhiteSpace(fila.MotivoRiesgo));
    }

    [TestMethod]
    public void RiesgoPage_SinDatos_ExponeEstadoSinInformacion()
    {
        CrearEstudiante("VR101", "Sofía");

        var fila = CrearRiesgo().CalcularRiesgos("1K1").Single();

        Assert.IsTrue(fila.SinInformacion);
        Assert.AreEqual("Sin información", fila.Estado);
        Assert.AreEqual("No hay información suficiente para evaluar.", fila.MotivoRiesgo);
    }
}
