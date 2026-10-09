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
    }
}