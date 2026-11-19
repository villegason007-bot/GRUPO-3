using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class ReporteRiesgoPage : ContentPage
    {
        public ReporteRiesgoPage(ReporteRiesgoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ReporteRiesgoViewModel vm)
            {
                vm.Inicializar();
            }
        }
    }
}
