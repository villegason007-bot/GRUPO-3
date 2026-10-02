using Avalonia.Controls;
using Avalonia.Interactivity;

namespace IU.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void BtnIngresar_Click(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is MainWindow mainWindow)
        {
            mainWindow.MostrarDashboard();
        }
    }
}