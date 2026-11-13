using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test.Aceptacion.Vistas;

[TestClass]
[DoNotParallelize]
public sealed class VistasCursadaAcceptanceTests : RepositorioTestBase
{
    [TestMethod]
    public void CalendarioPage_ConsultarCalendario_MuestraClasesYTrabajosOrdenados()
    {
        var fechaClase = DateTime.Today.AddDays(-14);
        var fechaTrabajo = DateTime.Today.AddDays(-7);
        new ClaseRepository().AgregarClase(new Clase { Fecha = fechaClase }, "1K1");
        new TrabajoRepository().AgregarTrabajo(
            new Trabajo { Titulo = "TP de vista", FechaDeEntrega = fechaTrabajo }, "1K1");

        var eventos = new CalendarioService(new CalendarioRepository())
            .VerCalendario("1K1");

        Assert.AreEqual(2, eventos.Count);
        Assert.AreEqual("CLASE", eventos[0].Tipo);
        Assert.AreEqual("TRABAJO", eventos[1].Tipo);
        Assert.AreEqual(fechaClase, eventos[0].Fecha);
        Assert.AreEqual(fechaTrabajo, eventos[1].Fecha);
    }

    [TestMethod]
    public void AsistenciaEntregasPage_GuardarAsistenciaYCorregirla_MuestraElEstadoActual()
    {
        var estudianteId = CrearEstudiante("VC100", "Nora");
        var claseId = CrearClase(DateTime.Today.AddDays(-7));
        var servicio = new AsistenciaService(new AsistenciaRepository());

        servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Ausente);
        servicio.GuardarAsistencia(claseId, estudianteId, Condicion.Presente);

        var filas = servicio.ListarPorClase(claseId);
        Assert.AreEqual(1, filas.Count);
        Assert.AreEqual(Condicion.Presente, filas[0].Condicion);
    }

    [TestMethod]
    public void AsistenciaEntregasPage_GuardarEntregaYCorregirla_MuestraFechaSoloCuandoEntrego()
    {
        var estudianteId = CrearEstudiante("VC101", "Oscar");
        var trabajoId = CrearTrabajo(DateTime.Today.AddDays(-7));
        var fechaEntrega = DateTime.Today.AddDays(-2);
        var servicio = new EntregaService(new EntregaRepository());

        servicio.GuardarEntrega(trabajoId, estudianteId, false, DateTime.Today);
        Assert.IsFalse(servicio.ListarPorTrabajo(trabajoId).Single().Entregado);
        Assert.IsNull(servicio.ListarPorTrabajo(trabajoId).Single().Fecha);

        servicio.GuardarEntrega(trabajoId, estudianteId, true, fechaEntrega);
        var entrega = servicio.ListarPorTrabajo(trabajoId).Single();
        Assert.IsTrue(entrega.Entregado);
        Assert.AreEqual(fechaEntrega, entrega.Fecha);
    }

    [TestMethod]
    public void MateriasPage_CrearYAsignarMateria_LaMuestraEnLaComision()
    {
        var servicio = new MateriaService(
            new MateriaRepository(), new ComisionRepository());

        servicio.CrearMateria("Programación");
        servicio.AsignarMateria("Programación", "1K1");

        var materias = servicio.ListarMateriasDeComision("1K1");
        Assert.AreEqual(1, materias.Count);
        Assert.AreEqual("Programación", materias[0].Nombre);
    }
}
