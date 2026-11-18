using Gestor_de_Estudiantes.ViewModel;

namespace Gestor_de_Estudiantes.Views
{
    /// <summary>
    /// Historial de entregas e indicador de riesgo individual del
    /// estudiante (H13). Recibe el legajo por la ruta (parámetro
    /// «legajo»).
    /// </summary>
    [QueryProperty(nameof(Legajo), "legajo")]
    public partial class HistorialEntregasPage : ContentPage
    {
        private readonly HistorialEntregasViewModel _vm;
        private string _legajo = string.Empty;

        public HistorialEntregasPage(HistorialEntregasViewModel vm)
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
