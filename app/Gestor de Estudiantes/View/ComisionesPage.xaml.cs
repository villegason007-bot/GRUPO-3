using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.Views;

public partial class ComisionesPage : ContentPage
{
    private readonly ComisionService _comisionService;
    private readonly EstudianteService _estudianteService;

    private List<Comision> _comisiones = new();

    public ComisionesPage(
        ComisionService comisionService,
        EstudianteService estudianteService)
    {
        InitializeComponent();

        _comisionService = comisionService;
        _estudianteService = estudianteService;

        CargarComisiones();
    }

    private void CargarComisiones()
    {
        try
        {
            _comisiones = _comisionService.ListarComisiones();

            ComisionesList.ItemsSource = _comisiones;
            ComisionFiltroPicker.ItemsSource = _comisiones;
            ComisionEstudiantePicker.ItemsSource = _comisiones;

            SinComisionesLabel.IsVisible = _comisiones.Count == 0;

            if (_comisiones.Count > 0)
            {
                ComisionFiltroPicker.SelectedIndex = 0;
            }
            else
            {
                EstudiantesList.ItemsSource = null;
                TituloNominaLabel.Text = "Estudiantes";
                SinEstudiantesLabel.Text =
                    "Primero tenés que crear una comisión.";
                SinEstudiantesLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            MostrarError(
                "No se pudieron cargar las comisiones.",
                ex);
        }
    }

    private void CargarEstudiantes(string codigoComision)
    {
        try
        {
            var estudiantes =
                _estudianteService.ListarEstudiantes(codigoComision);

            EstudiantesList.ItemsSource = estudiantes;

            TituloNominaLabel.Text =
                $"Estudiantes de {codigoComision}";

            SinEstudiantesLabel.Text = estudiantes.Count == 0
                ? "Esta comisión todavía no tiene estudiantes."
                : string.Empty;

            SinEstudiantesLabel.IsVisible =
                estudiantes.Count == 0;
        }
        catch (Exception ex)
        {
            EstudiantesList.ItemsSource = null;
            SinEstudiantesLabel.Text =
                "No se pudo cargar la nómina.";
            SinEstudiantesLabel.IsVisible = true;

            MostrarError(
                "No se pudieron cargar los estudiantes.",
                ex);
        }
    }

    private void ComisionFiltro_Changed(
        object sender,
        EventArgs e)
    {
        if (ComisionFiltroPicker.SelectedItem is Comision comision)
        {
            CargarEstudiantes(comision.Codigo);
        }
        else
        {
            EstudiantesList.ItemsSource = null;
            TituloNominaLabel.Text = "Estudiantes";
            SinEstudiantesLabel.Text =
                "Seleccioná una comisión para consultar la nómina.";
            SinEstudiantesLabel.IsVisible = true;
        }
    }

    private void ConsultarComision_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is Button boton &&
            boton.CommandParameter is Comision comision)
        {
            ComisionFiltroPicker.SelectedItem =
                _comisiones.FirstOrDefault(c => c.Id == comision.Id);

            CargarEstudiantes(comision.Codigo);
        }
    }

    private void AbrirModalComision_Clicked(
        object sender,
        EventArgs e)
    {
        CodigoComisionEntry.Text = string.Empty;
        SemanasEntry.Text = "16";

        ErrorComisionLabel.Text = string.Empty;
        ErrorComisionLabel.IsVisible = false;

        ModalEstudiante.IsVisible = false;
        ModalComision.IsVisible = true;
    }

    private void AbrirModalEstudiante_Clicked(
        object sender,
        EventArgs e)
    {
        if (_comisiones.Count == 0)
        {
            MostrarAviso(
                "Primero tenés que crear una comisión.");
            return;
        }

        LegajoEntry.Text = string.Empty;
        NombreEstudianteEntry.Text = string.Empty;

        ErrorEstudianteLabel.Text = string.Empty;
        ErrorEstudianteLabel.IsVisible = false;

        // Usar la comisión que se está consultando.
        ComisionEstudiantePicker.SelectedItem =
            ComisionFiltroPicker.SelectedItem
            ?? _comisiones.FirstOrDefault();

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
        var codigo = CodigoComisionEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            MostrarErrorModal(
                ErrorComisionLabel,
                "Ingresá el código de la comisión.");
            return;
        }

        if (!int.TryParse(SemanasEntry.Text, out var semanas)
            || semanas < 1)
        {
            MostrarErrorModal(
                ErrorComisionLabel,
                "La duración debe ser de al menos una semana.");
            return;
        }

        try
        {
            _comisionService.AgregarComision(codigo, semanas);

            ModalComision.IsVisible = false;

            CargarComisiones();

            MostrarAviso("La comisión se creó correctamente.");
        }
        catch (ArgumentException ex)
        {
            MostrarErrorModal(ErrorComisionLabel, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            MostrarErrorModal(ErrorComisionLabel, ex.Message);
        }
        catch (Exception ex)
        {
            MostrarErrorModal(
                ErrorComisionLabel,
                $"No se pudo guardar la comisión: {ex.Message}");
        }
    }

    private void GuardarEstudiante_Clicked(
        object sender,
        EventArgs e)
    {
        var legajo = LegajoEntry.Text?.Trim();
        var nombre = NombreEstudianteEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(legajo))
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                "Ingresá el legajo del estudiante.");
            return;
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                "Ingresá el nombre del estudiante.");
            return;
        }

        if (ComisionEstudiantePicker.SelectedItem
            is not Comision comision)
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                "Seleccioná una comisión.");
            return;
        }

        try
        {
            _estudianteService.AgregarEstudiante(
                legajo,
                nombre,
                comision.Codigo);

            ModalEstudiante.IsVisible = false;

            // Refrescar la nómina de la comisión elegida.
            ComisionFiltroPicker.SelectedItem =
                _comisiones.FirstOrDefault(
                    c => c.Id == comision.Id);

            CargarEstudiantes(comision.Codigo);

            MostrarAviso(
                "El estudiante se agregó correctamente.");
        }
        catch (ArgumentException ex)
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                ex.Message);
        }
        catch (Exception ex)
        {
            MostrarErrorModal(
                ErrorEstudianteLabel,
                $"No se pudo guardar el estudiante: {ex.Message}");
        }
    }

    private static void MostrarErrorModal(
        Label etiqueta,
        string mensaje)
    {
        etiqueta.Text = mensaje;
        etiqueta.IsVisible = true;
    }

    private async void MostrarAviso(string mensaje)
    {
        await DisplayAlert("Gestión de estudiantes", mensaje, "Aceptar");
    }

    private async void MostrarError(
        string mensaje,
        Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(ex);

        await DisplayAlert(
            "Error",
            $"{mensaje}\n{ex.Message}",
            "Aceptar");
    }

    private async void Volver_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }


    private void MostrarComisionesTab_Clicked(object sender, EventArgs e)
    {
        ComisionesPanel.IsVisible = true;
        EstudiantesPanel.IsVisible = false;

        TabComisionesButton.BackgroundColor = Color.FromArgb("#7667F5");
        TabComisionesButton.TextColor = Colors.White;

        TabEstudiantesButton.BackgroundColor = Colors.Transparent;
        TabEstudiantesButton.TextColor = Color.FromArgb("#AEBBD0");
    }

    private void MostrarEstudiantesTab_Clicked(object sender, EventArgs e)
    {
        ComisionesPanel.IsVisible = false;
        EstudiantesPanel.IsVisible = true;

        TabEstudiantesButton.BackgroundColor = Color.FromArgb("#7667F5");
        TabEstudiantesButton.TextColor = Colors.White;

        TabComisionesButton.BackgroundColor = Colors.Transparent;
        TabComisionesButton.TextColor = Color.FromArgb("#AEBBD0");

        // Si ya hay una comisión seleccionada, refrescar su nómina.
        if (ComisionFiltroPicker.SelectedItem is Comision comision)
        {
            CargarEstudiantes(comision.Codigo);
        }
    }

}