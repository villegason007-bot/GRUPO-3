using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    /// <summary>
    /// Historial de asistencia del estudiante (H12): clases con registro
    /// y condición de cada una. Recibe el legajo por la ruta
    /// (parámetro «legajo»).
    /// </summary>
    [QueryProperty(nameof(Legajo), "legajo")]
    public partial class HistorialAsistenciaPage : ContentPage
    {
        private readonly HistorialAsistenciaViewModel _vm;
        private string _legajo = string.Empty;

        public HistorialAsistenciaPage(HistorialAsistenciaViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        /// <summary>Legajo del estudiante a consultar (llega por la ruta).</summary>
        public string Legajo
        {
            get => _legajo;
            set
            {
                _legajo = value;
                _vm.CargarHistorial(value);
            }
        }
    }
}
