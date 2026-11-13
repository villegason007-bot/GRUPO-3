using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test.Aceptacion.Vistas;

[TestClass]
[DoNotParallelize]
public sealed class VistasComisionesAcceptanceTests : RepositorioTestBase
{
    [TestMethod]
    public void ComisionesPage_AgregarComision_LaMuestraEnElListado()
    {
        var servicio = new ComisionService(new ComisionRepository());

        var comision = servicio.AgregarComision("2K1", 12);
        var listado = servicio.ListarComisiones();

        Assert.AreEqual("2K1", comision.Codigo);
        Assert.AreEqual(12, comision.Semanas);
        Assert.IsTrue(listado.Any(item => item.Codigo == "2K1"));
    }

    [TestMethod]
    public void AgregarComisionPage_CodigoDuplicado_MuestraUnaValidacionDeNegocio()
    {
        var servicio = new ComisionService(new ComisionRepository());
        servicio.AgregarComision("2K2");

        Assert.Throws<InvalidOperationException>(
            () => servicio.AgregarComision(" 2k2 "));
    }

    [TestMethod]
    public void AgregarEstudiantePage_AltaCorrecta_ActualizaComisionDetalleYListado()
    {
        var servicio = new EstudianteService(new EstudianteRepository());

        servicio.AgregarEstudiante("V100", "Valentina", "1K1");

        var estudiantes = servicio.ListarEstudiantes("1K1");
        Assert.AreEqual(1, estudiantes.Count);
        Assert.AreEqual("Valentina", estudiantes[0].Nombre);
        Assert.AreEqual("V100", servicio.BuscarEstudiante("V100")!.Legajo);
    }

    [TestMethod]
    public void EstudiantesPage_DarDeBaja_ElEstudianteDejaDeAparecerPeroSeConservaElHistorial()
    {
        var servicio = new EstudianteService(new EstudianteRepository());
        servicio.AgregarEstudiante("V101", "Walter", "1K1");

        Assert.IsTrue(servicio.EliminarEstudiante("V101"));
        Assert.IsEmpty(servicio.ListarEstudiantes("1K1"));
        Assert.IsNull(servicio.BuscarEstudiante("V101"));
    }
}
