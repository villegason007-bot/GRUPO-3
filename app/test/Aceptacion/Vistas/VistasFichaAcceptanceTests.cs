using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test.Aceptacion.Vistas;

[TestClass]
[DoNotParallelize]
public sealed class VistasFichaAcceptanceTests : RepositorioTestBase
{
    [TestMethod]
    public void FichaEstudiantePage_SeleccionarEstudiante_MuestraDatosYUltimoContacto()
    {
        CrearEstudiante("VF100", "Tamara");
        new ContactoService(new ContactoRepository(), new EstudianteRepository())
            .RegistrarContacto("VF100", MedioContacto.Llamada, ResultadoContacto.Contesto);

        var ficha = new FichaService(
            new EstudianteRepository(), new ContactoRepository())
            .ObtenerFicha("VF100");

        Assert.AreEqual("VF100", ficha.Legajo);
        Assert.AreEqual("Tamara García", ficha.NombreCompleto);
        Assert.AreEqual(DateTime.Today, ficha.UltimoContacto!.Fecha);
    }

    [TestMethod]
    public void HistorialAsistenciaPage_SeleccionarEstudiante_MuestraLasAsistencias()
    {
        var estudianteId = CrearEstudiante("VF101", "Ulises");
        var fecha = DateTime.Today.AddDays(-7);
        var claseId = CrearClase(fecha);
        new AsistenciaService(new AsistenciaRepository())
            .GuardarAsistencia(claseId, estudianteId, Condicion.Presente);

        var historial = new HistorialAsistenciaService(
            new EstudianteRepository(), new AsistenciaRepository())
            .HistorialAsistencia("VF101");

        Assert.AreEqual(1, historial.Count);
        Assert.AreEqual(fecha, historial[0].Fecha);
        Assert.IsTrue(historial[0].EstaPresente);
    }

    [TestMethod]
    public void HistorialEntregasPage_SeleccionarEstudiante_MuestraEntregaYRiesgo()
    {
        var estudianteId = CrearEstudiante("VF102", "Valeria");
        var trabajoId = CrearTrabajo(DateTime.Today.AddDays(-7));
        var fechaEntrega = DateTime.Today.AddDays(-2);
        new EntregaService(new EntregaRepository())
            .GuardarEntrega(trabajoId, estudianteId, true, fechaEntrega);

        var entregas = new HistorialEntregasService(
            new EstudianteRepository(), new EntregaRepository())
            .HistorialEntregas("VF102");
        var riesgo = new RiesgoService(
            new RiesgoRepository(), new ParametrosRiesgoRepository(),
            new EstudianteRepository(), new CalculadoraDeRiesgo(),
            new ContactoRepository()).CalcularRiesgoDe("VF102");

        Assert.AreEqual(1, entregas.Count);
        Assert.IsTrue(entregas[0].Entregado);
        Assert.AreEqual(fechaEntrega, entregas[0].Fecha);
        Assert.AreEqual(0, riesgo.NoEntregados);
    }
}
