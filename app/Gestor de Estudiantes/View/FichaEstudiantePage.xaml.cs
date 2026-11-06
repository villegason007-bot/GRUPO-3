using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    /// <summary>
    /// Ficha del estudiante (H9): datos de contacto, historial de
    /// contactos (H10), modal para registrar uno nuevo y materias en las
    /// que está inscripto. Recibe el legajo por la ruta (parámetro «legajo»).
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

            _vm.MateriasInscriptas.CollectionChanged += (_, _) => ActualizarMateriasVacio();
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

            ActualizarMateriasVacio();
        }

        private void ActualizarMateriasVacio() =>
            SinMateriasLabel.IsVisible = _vm.MateriasInscriptas.Count == 0;

        // ---------------- Contactos (H10) ----------------

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

        // ---------------- Materias ----------------

        private void AbrirModalMateria_Clicked(object sender, EventArgs e)
        {
            MateriaPicker.ItemsSource = _vm.MateriasAsignables;
            MateriaPicker.SelectedItem = null;
            ModalMateria.LimpiarError();
            ModalMateria.IsVisible = true;
        }

        private void CancelarMateria_Clicked(object sender, EventArgs e)
        {
            ModalMateria.IsVisible = false;
        }

        private void AsignarMateria_Clicked(object sender, EventArgs e)
        {
            if (!_vm.InscribirEnMateria(MateriaPicker.SelectedItem as Materia, out var mensaje))
            {
                ModalMateria.MostrarError(mensaje);
                return;
            }

            ModalMateria.IsVisible = false;
        }

        private async void QuitarMateria_Clicked(object sender, EventArgs e)
        {
            if (((BindableObject)sender).BindingContext is not Materia materia)
                return;

            var confirmar = await DisplayAlert(
                "Quitar inscripción",
                $"¿Querés quitar la inscripción en «{materia.Nombre}»?",
                "Quitar",
                "Cancelar");

            if (confirmar)
                _vm.DesinscribirDeMateria(materia, out _);
        }

        // ---------------- Historiales ----------------

        // Historial de asistencia del estudiante (H12).
        private async void OnHistorialAsistenciaClicked(object sender, EventArgs e)
        {
            if (_legajo.Length == 0)
                return;

            await Shell.Current.GoToAsync(
                nameof(HistorialAsistenciaPage),
                new Dictionary<string, object>
                {
                    ["legajo"] = _legajo,
                });
        }

        // Historial de entregas con el riesgo individual (H13).
        private async void OnHistorialEntregasClicked(object sender, EventArgs e)
        {
            if (_legajo.Length == 0)
                return;

            await Shell.Current.GoToAsync(
                nameof(HistorialEntregasPage),
                new Dictionary<string, object>
                {
                    ["legajo"] = _legajo,
                });
        }
    }
}