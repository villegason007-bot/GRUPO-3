using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EstudiantesPage), typeof(EstudiantesPage));
            Routing.RegisterRoute(nameof(FichaEstudiantePage), typeof(FichaEstudiantePage));
            Routing.RegisterRoute(nameof(HistorialAsistenciaPage), typeof(HistorialAsistenciaPage));
            Routing.RegisterRoute(nameof(HistorialEntregasPage), typeof(HistorialEntregasPage));
            Routing.RegisterRoute(nameof(ComisionesPage), typeof(ComisionesPage));
            Routing.RegisterRoute(nameof(CalendarioPage), typeof(CalendarioPage));
            Routing.RegisterRoute(nameof(AsistenciaEntregasPage), typeof(AsistenciaEntregasPage));
            Routing.RegisterRoute(nameof(RiesgoPage), typeof(RiesgoPage));
            Routing.RegisterRoute(nameof(ConfiguracionRiesgoPage), typeof(ConfiguracionRiesgoPage));
            Routing.RegisterRoute(nameof(ComisionDetallePage), typeof(ComisionDetallePage));
            Routing.RegisterRoute(nameof(MateriasPage), typeof(MateriasPage));
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

        private async void OnVerEstudiantesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EstudiantesPage));
        }

        private async void OnVerMateriasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MateriasPage));
        }
    }
}