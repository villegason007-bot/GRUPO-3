
using Gestor_de_Estudiantes.ViewModel;
using Gestor_de_Estudiantes.Resources;

namespace Gestor_de_Estudiantes.Views;

public partial class CalendarioPage : ContentPage
{
    public CalendarioPage(CalendarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void MostrarCalendarioTab_Clicked(object sender, EventArgs e)
    {
        MostrarPanelCalendario("calendario");
    }

    private void MostrarClaseTab_Clicked(object sender, EventArgs e)
    {
        MostrarPanelCalendario("clase");
    }

    private void MostrarTrabajoTab_Clicked(object sender, EventArgs e)
    {
        MostrarPanelCalendario("trabajo");
    }

    private void MostrarPanelCalendario(string panel)
    {
        CalendarioPanel.IsVisible = panel == "calendario";
        ClasePanel.IsVisible = panel == "clase";
        TrabajoPanel.IsVisible = panel == "trabajo";

        TabCalendarioButton.BackgroundColor =
            panel == "calendario"
                ? Paleta.Acento
                : Colors.Transparent;

        TabClaseButton.BackgroundColor =
            panel == "clase"
                ? Paleta.Acento
                : Colors.Transparent;

        TabTrabajoButton.BackgroundColor =
            panel == "trabajo"
                ? Paleta.Acento
                : Colors.Transparent;

        TabCalendarioButton.TextColor =
            panel == "calendario"
                ? Colors.White
                : Paleta.TextoSecundario;

        TabClaseButton.TextColor =
            panel == "clase"
                ? Colors.White
                : Paleta.TextoSecundario;

        TabTrabajoButton.TextColor =
            panel == "trabajo"
                ? Colors.White
                : Paleta.TextoSecundario;
    }
}
