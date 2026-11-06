using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views;

/// <summary>
/// Listado de comisiones. Al elegir una se abre <see cref="ComisionDetallePage"/>,
/// que muestra sus materias asignadas y sus estudiantes.
/// </summary>
public partial class ComisionesPage : ContentPage
{
    private readonly ComisionesViewModel _vm;

    public ComisionesPage(ComisionesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        ComisionesList.ItemsSource = _vm.Comisiones;
        _vm.Comisiones.CollectionChanged += (_, _) => ActualizarVacio();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ComisionesList.SelectedItem = null;
        _vm.CargarComisiones();
        ActualizarVacio();
    }

    private void ActualizarVacio() =>
        SinComisionesLabel.IsVisible = _vm.Comisiones.Count == 0;

    private async void ComisionesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Comision comision)
            return;

        _vm.ComisionSeleccionada = comision;
        await Shell.Current.GoToAsync(nameof(ComisionDetallePage));
    }

    private void AbrirModalComision_Clicked(object sender, EventArgs e)
    {
        CodigoComisionEntry.Text = string.Empty;
        SemanasEntry.Text = Comision.SemanasPorDefecto.ToString();
        ModalComision.LimpiarError();
        ModalComision.IsVisible = true;
    }

    private void CancelarModal_Clicked(object sender, EventArgs e) =>
        ModalComision.IsVisible = false;

    private void GuardarComision_Clicked(object sender, EventArgs e)
    {
        if (!_vm.AgregarComision(CodigoComisionEntry.Text, SemanasEntry.Text))
        {
            ModalComision.MostrarError(_vm.Mensaje);
            return;
        }

        ModalComision.IsVisible = false;
        Aviso.Mostrar(_vm.Mensaje);
    }
}
