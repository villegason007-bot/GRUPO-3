using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class EstudiantesPage : ContentPage
    {
        private readonly EstudiantesViewModel _vm;

        public EstudiantesPage(EstudiantesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            _vm.MostrarEstudiantes();
        }

        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            var modal = new AgregarEstudiantePage(_vm);
            await Navigation.PushModalAsync(modal);
        }
    }
}