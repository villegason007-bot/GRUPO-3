
using Gestor_de_Estudiantes.ViewModel;
using System.Linq;

namespace Gestor_de_Estudiantes.Views
{
    public partial class RiesgoPage : ContentPage
    {
        public RiesgoPage(RiesgoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void Estudiantes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault()
                is not RiesgoFilaViewModel estudiante ||
                string.IsNullOrWhiteSpace(estudiante.Legajo))
                return;

            if (sender is CollectionView lista)
                lista.SelectedItem = null;

            await Shell.Current.GoToAsync(
                nameof(FichaEstudiantePage),
                new Dictionary<string, object>
                {
                    ["legajo"] = estudiante.Legajo,
                });
        }
    }
}
