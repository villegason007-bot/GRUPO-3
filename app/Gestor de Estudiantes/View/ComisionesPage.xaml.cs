using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;
using Gestor_de_Estudiantes.Resources;

namespace Gestor_de_Estudiantes.Views;

/// <summary>
/// Pestaña de comisiones y nómina. La lógica (carga, validación y guardado)
/// vive en <see cref="ComisionesViewModel"/>; el code-behind solo delega y
/// sincroniza los controles.
/// </summary>
public partial class ComisionesPage : ContentPage
{
    private readonly ComisionesViewModel _vm;

    public ComisionesPage(ComisionesViewModel vm)
    {
        InitializeComponent();

        _vm = vm;

        var seleccionada = _vm.ComisionSeleccionada;

        SincronizarPickers();
        ComisionFiltroPicker.SelectedItem = seleccionada;
        _vm.ComisionSeleccionada = seleccionada;

        ActualizarNomina();
    }

    /// <summary>Publica la colección del ViewModel en la lista y los pickers (se actualizan solos al cambiar).</summary>
    private void SincronizarPickers()
    {
        ComisionesList.ItemsSource = _vm.Comisiones;
        ComisionFiltroPicker.ItemsSource = _vm.Comisiones;
        ComisionEstudiantePicker.ItemsSource = _vm.Comisiones;

        SinComisionesLabel.IsVisible = _vm.Comisiones.Count == 0;
    }

    /// <summary>Refresca la nómina mostrada según la comisión seleccionada en el ViewModel.</summary>
    private void ActualizarNomina()
    {
        if (_vm.ComisionSeleccionada is null)
        {
            EstudiantesList.ItemsSource = null;
            TituloNominaLabel.Text = "Estudiantes";
            SinEstudiantesLabel.Text = _vm.Comisiones.Count == 0
                ? "Primero tenés que crear una comisión."
                : "Seleccioná una comisión para consultar la nómina.";
            SinEstudiantesLabel.IsVisible = true;
            return;
        }

        EstudiantesList.ItemsSource = _vm.Estudiantes;
        TituloNominaLabel.Text =
            $"Estudiantes de {_vm.ComisionSeleccionada.Codigo}";
        SinEstudiantesLabel.Text = _vm.Estudiantes.Count == 0
            ? "Esta comisión todavía no tiene estudiantes."
            : string.Empty;
        SinEstudiantesLabel.IsVisible = _vm.Estudiantes.Count == 0;
    }

    private void ComisionFiltro_Changed(
        object sender,
        EventArgs e)
    {
        _vm.ComisionSeleccionada =
            ComisionFiltroPicker.SelectedItem as Comision;

        ActualizarNomina();
    }

    private void AbrirModalComision_Clicked(
        object sender,
        EventArgs e)
    {
        CodigoComisionEntry.Text = string.Empty;
        SemanasEntry.Text = Comision.SemanasPorDefecto.ToString();

        ModalComision.LimpiarError();

        ModalEstudiante.IsVisible = false;
        ModalComision.IsVisible = true;
    }

    private void AbrirModalEstudiante_Clicked(
        object sender,
        EventArgs e)
    {
        if (_vm.Comisiones.Count == 0)
        {
            MostrarAviso(
                "Primero tenés que crear una comisión.");
            return;
        }

        LegajoEntry.Text = string.Empty;
        NombreEstudianteEntry.Text = string.Empty;

        ModalEstudiante.LimpiarError();

        // Usar la comisión que se está consultando.
        ComisionEstudiantePicker.SelectedItem =
            ComisionFiltroPicker.SelectedItem
            ?? _vm.Comisiones.FirstOrDefault();

        ModalComision.IsVisible = false;
        ModalEstudiante.IsVisible = true;
    }

    private void CancelarModal_Clicked(
        object sender,
        EventArgs e)
    {
        ModalComision.IsVisible = false;
        ModalEstudiante.IsVisible = false;
    }

    private void GuardarComision_Clicked(
        object sender,
        EventArgs e)
    {
        var guardada = _vm.AgregarComision(
            CodigoComisionEntry.Text,
            SemanasEntry.Text);

        if (!guardada)
        {
            ModalComision.MostrarError(_vm.Mensaje);
            return;
        }

        ModalComision.IsVisible = false;

        SinComisionesLabel.IsVisible = false;
        ComisionFiltroPicker.SelectedItem =
            _vm.ComisionSeleccionada;

        MostrarAviso(_vm.Mensaje);
    }

    private void GuardarEstudiante_Clicked(
        object sender,
        EventArgs e)
    {
        var guardado = _vm.AgregarEstudiante(
            LegajoEntry.Text,
            NombreEstudianteEntry.Text,
            ComisionEstudiantePicker.SelectedItem as Comision);

        if (!guardado)
        {
            ModalEstudiante.MostrarError(_vm.Mensaje);
            return;
        }

        ModalEstudiante.IsVisible = false;

        // Refrescar la nómina de la comisión elegida.
        ComisionFiltroPicker.SelectedItem =
            _vm.ComisionSeleccionada;

        ActualizarNomina();

        MostrarAviso(_vm.Mensaje);
    }

    private async void MostrarAviso(string mensaje)
    {
        await DisplayAlert("Gestión de estudiantes", mensaje, "Aceptar");
    }

    private void MostrarComisionesTab_Clicked(object sender, EventArgs e)
    {
        ComisionesPanel.IsVisible = true;
        EstudiantesPanel.IsVisible = false;

        TabComisionesButton.BackgroundColor = Paleta.Acento;
        TabComisionesButton.TextColor = Colors.White;

        TabEstudiantesButton.BackgroundColor = Colors.Transparent;
        TabEstudiantesButton.TextColor = Paleta.TextoSecundario;
    }

    private void MostrarEstudiantesTab_Clicked(object sender, EventArgs e)
    {
        ComisionesPanel.IsVisible = false;
        EstudiantesPanel.IsVisible = true;

        TabEstudiantesButton.BackgroundColor = Paleta.Acento;
        TabEstudiantesButton.TextColor = Colors.White;

        TabComisionesButton.BackgroundColor = Colors.Transparent;
        TabComisionesButton.TextColor = Paleta.TextoSecundario;

        // Si ya hay una comisión seleccionada, refrescar su nómina.
        if (ComisionFiltroPicker.SelectedItem is Comision comision)
        {
            _vm.ComisionSeleccionada = comision;
            ActualizarNomina();
        }
    }
}
