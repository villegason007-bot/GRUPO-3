using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnVerEstudiantesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EstudiantesPage));
        }
        private async void OnVerComisionesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ComisionesPage));
        }


        private async void OnCalendarioClicked(
            object sender,
            EventArgs e)
        {
            await Shell.Current.GoToAsync(
                nameof(Gestor_de_Estudiantes.Views.CalendarioPage));
        }


        private async void OnAsistenciaEntregasClicked(
            object sender,
            EventArgs e)
        {
            await Shell.Current.GoToAsync(
                nameof(Gestor_de_Estudiantes.Views.AsistenciaEntregasPage));
        }
        private async void OnRiesgoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RiesgoPage));
        }

        private async void OnConfiguracionRiesgoClicked(
            object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracionRiesgoPage));
        }
        

        private async void OnInicioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

    }
}