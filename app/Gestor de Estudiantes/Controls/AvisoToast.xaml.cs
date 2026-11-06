namespace Gestor_de_Estudiantes.Controls;

/// <summary>Aviso breve (3 s) para confirmar o informar el resultado de una acción.</summary>
public partial class AvisoToast : ContentView
{
    private int _ultimo;

    public AvisoToast() => InitializeComponent();

    public async void Mostrar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return;

        var token = ++_ultimo;
        AvisoLabel.Text = texto;
        IsVisible = true;

        await Task.Delay(3000);

        if (token == _ultimo)
            IsVisible = false;
    }
}
