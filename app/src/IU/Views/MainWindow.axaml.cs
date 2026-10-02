using Avalonia.Controls;

namespace IU.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MostrarLogin();
    }

    private void MostrarLogin()
    {
        MainContent.Content = new LoginView();
    }

    public void MostrarDashboard()
    {
        MainContent.Content = new DashboardView();
    }
}