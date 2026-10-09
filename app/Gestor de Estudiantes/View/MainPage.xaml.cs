using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _vm;
        public MainPage()
        {
            InitializeComponent();
            this._vm = new MainViewModel();
            this.BindingContext = this._vm;
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            this._vm.MostrarEstudiantes();
        }
    }
}
