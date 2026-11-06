using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views;

/// <summary>
/// Estudiantes filtrados por comisión. Al tocar uno se abre su ficha
/// (<see cref="FichaEstudiantePage"/>), donde también se le asignan materias.
/// </summary>
public partial class EstudiantesPage : ContentPage
{
    private readonly ComisionesViewModel _vm;

    public EstudiantesPage(ComisionesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;

        ComisionFiltroPicker.ItemsSource = _vm.Comisiones;
        ComisionEstudiantePicker.ItemsSource = _vm.Comisiones;
        EstudiantesList.ItemsSource = _vm.Estudiantes;

        _vm.Estudiantes.CollectionChanged += (_, _) => ActualizarNomina();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EstudiantesList.SelectedItem = null;

        _vm.CargarComisiones();
        ComisionFiltroPicker.SelectedItem = _vm.ComisionSeleccionada;
        ActualizarNomina();
    }

    private void ActualizarNomina()
    {
        var comision = _vm.ComisionSeleccionada;

        if (comision is null)
        {
            TituloNominaLabel.Text = "Estudiantes";
            SinEstudiantesLabel.Text = _vm.Comisiones.Count == 0
                ? "Primero tenés que crear una comisión."
                : "Seleccioná una comisión para consultar la nómina.";
            SinEstudiantesLabel.IsVisible = true;
            return;
        }

        TituloNominaLabel.Text = $"Estudiantes de {comision.Codigo}";
        SinEstudiantesLabel.Text = "Esta comisión todavía no tiene estudiantes.";
        SinEstudiantesLabel.IsVisible = _vm.Estudiantes.Count == 0;
    }

    private void ComisionFiltro_Changed(object sender, EventArgs e)
    {
        if (ComisionFiltroPicker.SelectedItem is Comision comision &&
            !ReferenceEquals(comision, _vm.ComisionSeleccionada))
        {
            _vm.ComisionSeleccionada = comision;
        }

        ActualizarNomina();
    }

    private async void EstudiantesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Estudiante estudiante)
            return;

        await Shell.Current.GoToAsync(
            nameof(FichaEstudiantePage),
            new Dictionary<string, object> { ["legajo"] = estudiante.Legajo });
    }

    private void AbrirModalEstudiante_Clicked(object sender, EventArgs e)
    {
        if (_vm.Comisiones.Count == 0)
        {
            Aviso.Mostrar("Primero tenés que crear una comisión.");
            return;
        }

        LegajoEntry.Text = string.Empty;
        NombreEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        ComisionEstudiantePicker.SelectedItem =
            _vm.ComisionSeleccionada ?? _vm.Comisiones.FirstOrDefault();
        ModalEstudiante.LimpiarError();
        ModalEstudiante.IsVisible = true;
    }

    private void CancelarModal_Clicked(object sender, EventArgs e) =>
        ModalEstudiante.IsVisible = false;

    private void GuardarEstudiante_Clicked(object sender, EventArgs e)
    {
        if (!_vm.AgregarEstudiante(new DatosNuevoEstudiante
        {
            Legajo = LegajoEntry.Text,
            Nombre = NombreEntry.Text,
            Telefono = TelefonoEntry.Text,
        }))
        {
            ModalEstudiante.MostrarError(_vm.Mensaje);
            return;
        }

        ModalEstudiante.IsVisible = false;
        ComisionFiltroPicker.SelectedItem = _vm.ComisionSeleccionada;
        ActualizarNomina();
        Aviso.Mostrar(_vm.Mensaje);
    }
}
