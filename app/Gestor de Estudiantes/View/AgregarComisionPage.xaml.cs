using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class AgregarComisionPage : ContentPage
    {
        private readonly ComisionesViewModel _vm;

        public AgregarComisionPage(ComisionesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (!int.TryParse(SemanasEntry.Text, out int semanas))
            {
                ErrorLabel.Text = "Las semanas deben ser un número entero.";
                ErrorLabel.IsVisible = true;
                return;
            }

            try
            {
                _vm.AgregarComision(CodigoEntry.Text, semanas);
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