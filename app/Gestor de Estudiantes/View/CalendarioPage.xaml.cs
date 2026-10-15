
using Gestor_de_Estudiantes.ViewModel;

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
                ? Color.FromArgb("#7667F5")
                : Colors.Transparent;

        TabClaseButton.BackgroundColor =
            panel == "clase"
                ? Color.FromArgb("#7667F5")
                : Colors.Transparent;

        TabTrabajoButton.BackgroundColor =
            panel == "trabajo"
                ? Color.FromArgb("#7667F5")
                : Colors.Transparent;

        TabCalendarioButton.TextColor =
            panel == "calendario"
                ? Colors.White
                : Color.FromArgb("#AEBBD0");

        TabClaseButton.TextColor =
            panel == "clase"
                ? Colors.White
                : Color.FromArgb("#AEBBD0");

        TabTrabajoButton.TextColor =
            panel == "trabajo"
                ? Colors.White
                : Color.FromArgb("#AEBBD0");
    }
}
