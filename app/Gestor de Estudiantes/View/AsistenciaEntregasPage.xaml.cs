
using Gestor_de_Estudiantes.ViewModel;
using Gestor_de_Estudiantes.Resources;

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

            TabAsistenciaButton.BackgroundColor = Paleta.Acento;
            TabAsistenciaButton.TextColor = Colors.White;

            TabEntregasButton.BackgroundColor = Colors.Transparent;
            TabEntregasButton.TextColor = Paleta.TextoSecundario;
        }

        private void MostrarEntregasTab_Clicked(object sender, EventArgs e)
        {
            AsistenciaPanel.IsVisible = false;
            EntregasPanel.IsVisible = true;

            TabEntregasButton.BackgroundColor = Paleta.Acento;
            TabEntregasButton.TextColor = Colors.White;

            TabAsistenciaButton.BackgroundColor = Colors.Transparent;
            TabAsistenciaButton.TextColor = Paleta.TextoSecundario;
        }

    }
}
