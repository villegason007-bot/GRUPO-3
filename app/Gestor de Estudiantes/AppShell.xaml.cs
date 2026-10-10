using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EstudiantesPage), typeof(EstudiantesPage));
            Routing.RegisterRoute(nameof(ComisionesPage), typeof(ComisionesPage));
        }
    }
}