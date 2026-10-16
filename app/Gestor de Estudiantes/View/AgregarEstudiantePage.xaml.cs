using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class AgregarEstudiantePage : ContentPage
    {
        private readonly EstudiantesViewModel _vm;

        public AgregarEstudiantePage(EstudiantesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                _vm.AgregarEstudiante(LegajoEntry.Text, NombreEntry.Text);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                ErrorLabel.Text = ex.Message;
                ErrorLabel.IsVisible = true;
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}