using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views;

/// <summary>
/// Detalle de la comisión elegida: materias asignadas (asignar / quitar) y
/// nómina de estudiantes (agregar / abrir ficha).
/// Los estudiantes salen de <see cref="ComisionesViewModel"/> y las materias
/// de <see cref="ComisionMateriasViewModel"/>.
/// </summary>
public partial class ComisionDetallePage : ContentPage
{
    private readonly ComisionesViewModel _vm;
    private readonly ComisionMateriasViewModel _materias;

    public ComisionDetallePage(ComisionesViewModel vm, ComisionMateriasViewModel materias)
    {
        InitializeComponent();
        _vm = vm;
        _materias = materias;
        BindingContext = _vm;

        BindableLayout.SetItemsSource(MateriasLayout, _materias.MateriasComision);

        _materias.MateriasComision.CollectionChanged += (_, _) => ActualizarVacios();
        _vm.Estudiantes.CollectionChanged += (_, _) => ActualizarVacios();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Se refresca por si hubo cambios desde otra vista (materias, estudiantes, ficha).
        _materias.Cargar(_vm.ComisionSeleccionada);
        _vm.CargarEstudiantes();
        ActualizarVacios();
    }

    private void ActualizarVacios()
    {
        SinMateriasLabel.IsVisible = _materias.MateriasComision.Count == 0;
        SinEstudiantesLabel.IsVisible = _vm.Estudiantes.Count == 0;
    }

    private async void Volver_Clicked(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");

    // ---------------- Materias ----------------

    private void AbrirModalMateria_Clicked(object sender, EventArgs e)
    {
        MateriaPicker.ItemsSource = _materias.MateriasDisponibles;
        MateriaPicker.SelectedItem = null;
        SinDisponiblesLabel.IsVisible = _materias.MateriasDisponibles.Count == 0;
        ModalMateria.LimpiarError();
        ModalMateria.IsVisible = true;
    }

    private void AsignarMateria_Clicked(object sender, EventArgs e)
    {
        if (!_materias.Asignar(_vm.ComisionSeleccionada, MateriaPicker.SelectedItem as Materia))
        {
            ModalMateria.MostrarError(_materias.Mensaje);
            return;
        }

        ModalMateria.IsVisible = false;
        Aviso.Mostrar(_materias.Mensaje);
    }

    private async void QuitarMateria_Clicked(object sender, EventArgs e)
    {
        if (((BindableObject)sender).BindingContext is not Materia materia)
            return;

        var confirmar = await DisplayAlert(
            "Quitar materia",
            $"¿Querés quitar la materia «{materia.Nombre}» de esta comisión?",
            "Quitar",
            "Cancelar");

        if (!confirmar)
            return;

        _materias.Desasignar(_vm.ComisionSeleccionada, materia);
        Aviso.Mostrar(_materias.Mensaje);
    }

    // ---------------- Estudiantes ----------------

    private void AbrirModalEstudiante_Clicked(object sender, EventArgs e)
    {
        LegajoEntry.Text = string.Empty;
        NombreEntry.Text = string.Empty;
        ModalEstudiante.LimpiarError();
        ModalEstudiante.IsVisible = true;
    }

    private void GuardarEstudiante_Clicked(object sender, EventArgs e)
    {
        if (!_vm.AgregarEstudiante(
                LegajoEntry.Text,
                NombreEntry.Text,
                _vm.ComisionSeleccionada))
        {
            ModalEstudiante.MostrarError(_vm.Mensaje);
            return;
        }

        ModalEstudiante.IsVisible = false;
        Aviso.Mostrar(_vm.Mensaje);
    }

    private async void Estudiante_Tapped(object sender, TappedEventArgs e)
    {
        if (((BindableObject)sender).BindingContext is not Estudiante estudiante)
            return;

        await Shell.Current.GoToAsync(
            nameof(FichaEstudiantePage),
            new Dictionary<string, object> { ["legajo"] = estudiante.Legajo });
    }

    private void CancelarModal_Clicked(object sender, EventArgs e)
    {
        ModalMateria.IsVisible = false;
        ModalEstudiante.IsVisible = false;
    }
}