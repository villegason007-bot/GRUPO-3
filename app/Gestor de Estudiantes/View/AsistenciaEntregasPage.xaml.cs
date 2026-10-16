
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class AsistenciaEntregasPage : ContentPage
    {
        public AsistenciaEntregasPage(
            AsistenciaEntregasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }


        private void MostrarAsistenciaTab_Clicked(object sender, EventArgs e)
        {
            AsistenciaPanel.IsVisible = true;
            EntregasPanel.IsVisible = false;

            TabAsistenciaButton.BackgroundColor = Color.FromArgb("#7667F5");
            TabAsistenciaButton.TextColor = Colors.White;

            TabEntregasButton.BackgroundColor = Colors.Transparent;
            TabEntregasButton.TextColor = Color.FromArgb("#AEBBD0");
        }

        private void MostrarEntregasTab_Clicked(object sender, EventArgs e)
        {
            AsistenciaPanel.IsVisible = false;
            EntregasPanel.IsVisible = true;

            TabEntregasButton.BackgroundColor = Color.FromArgb("#7667F5");
            TabEntregasButton.TextColor = Colors.White;

            TabAsistenciaButton.BackgroundColor = Colors.Transparent;
            TabAsistenciaButton.TextColor = Color.FromArgb("#AEBBD0");
        }

    }
}
