
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class ConfiguracionRiesgoPage : ContentPage
    {
        public ConfiguracionRiesgoPage(
            ConfiguracionRiesgoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
