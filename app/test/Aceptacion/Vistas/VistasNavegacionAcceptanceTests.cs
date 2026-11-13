using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;

namespace Test.Aceptacion.Vistas;

[TestClass]
[DoNotParallelize]
public sealed class VistasNavegacionAcceptanceTests : RepositorioTestBase
{
    [TestMethod]
    public void MainPage_AbrirEspacioDeTrabajo_PermiteAccederAComisionesYEstudiantes()
    {
        var comisiones = new ComisionService(new ComisionRepository()).ListarComisiones();
        var estudiantes = new EstudianteService(new EstudianteRepository())
            .ListarEstudiantes("1K1");

        Assert.IsNotNull(comisiones);
        Assert.IsNotNull(estudiantes);
        Assert.IsTrue(comisiones.Any(comision => comision.Codigo == "1K1"));
    }

    [TestMethod]
    public void ComisionDetallePage_SeleccionarComision_MuestraMateriasYEstudiantes()
    {
        var estudiantes = new EstudianteService(new EstudianteRepository());
        estudiantes.AgregarEstudiante("VN100", "Wanda", "1K1");
        var materias = new MateriaService(
            new MateriaRepository(), new ComisionRepository());
        materias.CrearMateria("Base de datos");
        materias.AsignarMateria("Base de datos", "1K1");

        var estudiantesDeComision = estudiantes.ListarEstudiantes("1K1");
        var materiasDeComision = materias.ListarMateriasDeComision("1K1");

        Assert.IsTrue(estudiantesDeComision.Any(e => e.Legajo == "VN100"));
        Assert.IsTrue(materiasDeComision.Any(m => m.Nombre == "Base de datos"));
    }
}
