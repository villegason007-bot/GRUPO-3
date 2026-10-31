using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    /// <summary>
    /// Ficha del estudiante (H9): datos de contacto, historial de
    /// contactos (H10) y modal para registrar uno nuevo. Recibe el
    /// legajo por la ruta (parámetro «legajo»).
    /// </summary>
    [QueryProperty(nameof(Legajo), "legajo")]
    public partial class FichaEstudiantePage : ContentPage
    {
        private readonly FichaEstudianteViewModel _vm;
        private string _legajo = string.Empty;

        public FichaEstudiantePage(FichaEstudianteViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        /// <summary>Legajo del estudiante a mostrar (llega por la ruta).</summary>
        public string Legajo
        {
            get => _legajo;
            set
            {
                _legajo = value;
                CargarFicha();
            }
        }

        private void CargarFicha()
        {
            _vm.CargarFicha(_legajo);

            // Las opciones de los pickers del modal salen del ViewModel.
            MedioPicker.ItemsSource = _vm.Medios;
            ResultadoPicker.ItemsSource = _vm.Resultados;
        }

        private void OnRegistrarContactoClicked(object sender, EventArgs e)
        {
            MedioPicker.SelectedItem = null;
            ResultadoPicker.SelectedItem = null;

            ModalContacto.LimpiarError();
            ModalContacto.IsVisible = true;
        }

        private void CancelarContacto_Clicked(object sender, EventArgs e)
        {
            ModalContacto.IsVisible = false;
        }

        private void GuardarContacto_Clicked(object sender, EventArgs e)
        {
            var error = _vm.RegistrarContacto(
                MedioPicker.SelectedItem as string,
                ResultadoPicker.SelectedItem as string);

            if (error != null)
            {
                ModalContacto.MostrarError(error);
                return;
            }

            ModalContacto.IsVisible = false;
        }
    }
}
