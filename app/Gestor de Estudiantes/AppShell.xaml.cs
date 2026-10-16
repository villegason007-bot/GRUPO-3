using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EstudiantesPage), typeof(EstudiantesPage));
            Routing.RegisterRoute(nameof(ComisionesPage), typeof(ComisionesPage));
            Routing.RegisterRoute(nameof(CalendarioPage), typeof(CalendarioPage));
            Routing.RegisterRoute(nameof(AsistenciaEntregasPage), typeof(AsistenciaEntregasPage));
            Routing.RegisterRoute(nameof(RiesgoPage), typeof(RiesgoPage));
            Routing.RegisterRoute(nameof(ConfiguracionRiesgoPage), typeof(ConfiguracionRiesgoPage));
        }

        private async void OnInicioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnVerComisionesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ComisionesPage));
        }

        private async void OnAsistenciaEntregasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AsistenciaEntregasPage));
        }

        private async void OnRiesgoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RiesgoPage));
        }

        private async void OnCalendarioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CalendarioPage));
        }

        private async void OnConfiguracionRiesgoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracionRiesgoPage));
        }
    }
}