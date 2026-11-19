using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views;

public partial class ReporteRiesgoPage : ContentPage
{
    private readonly ReporteRiesgoViewModel _vm;

    public ReporteRiesgoPage(ReporteRiesgoViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        _vm.Inicializar();
    }
}
