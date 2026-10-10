using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    public partial class ComisionesPage : ContentPage
    {
        private readonly ComisionesViewModel _vm;

        public ComisionesPage(ComisionesViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            _vm.MostrarComisiones();
        }

        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AgregarComisionPage(_vm));
        }
    }
}