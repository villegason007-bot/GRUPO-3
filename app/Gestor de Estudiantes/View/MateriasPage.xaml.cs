using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views;

/// <summary>Catálogo de materias: listar, crear y eliminar.</summary>
public partial class MateriasPage : ContentPage
{
    private readonly MateriasViewModel _vm;

    public MateriasPage(MateriasViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        MateriasList.ItemsSource = _vm.Materias;
        _vm.Materias.CollectionChanged += (_, _) => ActualizarVacio();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Cargar();
        ActualizarVacio();
    }

    private void ActualizarVacio() =>
        SinMateriasLabel.IsVisible = _vm.Materias.Count == 0;

    private void AbrirModalMateria_Clicked(object sender, EventArgs e)
    {
        NombreMateriaEntry.Text = string.Empty;
        ModalMateria.LimpiarError();
        ModalMateria.IsVisible = true;
    }

    private void CancelarModal_Clicked(object sender, EventArgs e) =>
        ModalMateria.IsVisible = false;

    private void GuardarMateria_Clicked(object sender, EventArgs e)
    {
        if (!_vm.Crear(NombreMateriaEntry.Text))
        {
            ModalMateria.MostrarError(_vm.Mensaje);
            return;
        }

        ModalMateria.IsVisible = false;
        Aviso.Mostrar(_vm.Mensaje);
    }

    private async void EliminarMateria_Clicked(object sender, EventArgs e)
    {
        if (((BindableObject)sender).BindingContext is not Materia materia)
            return;

        var confirmar = await DisplayAlert(
            "Eliminar materia",
            $"¿Querés eliminar «{materia.Nombre}»? Se quitará de todas las comisiones " +
            "y de las inscripciones de los estudiantes.",
            "Eliminar",
            "Cancelar");

        if (!confirmar)
            return;

        _vm.Eliminar(materia);
        Aviso.Mostrar(_vm.Mensaje);
    }
}
