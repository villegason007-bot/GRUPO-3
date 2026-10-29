using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.Controls;

/// <summary>
/// Mensaje de "lista vacía" centrado. Se usa como hijo de CollectionView.EmptyView
/// o como aviso dentro de los paneles, y expone <see cref="Text"/> para que el
/// code-behind pueda cambiar el mensaje igual que con un Label.
/// </summary>
public partial class EstadoVacio : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(EstadoVacio), string.Empty);

    public static readonly BindableProperty TamanoTextoProperty =
        BindableProperty.Create(nameof(TamanoTexto), typeof(double), typeof(EstadoVacio), 13d);

    public static readonly BindableProperty ColorTextoProperty =
        BindableProperty.Create(nameof(ColorTexto), typeof(Color), typeof(EstadoVacio),
            Color.FromArgb("#AEBBD0"));

    public EstadoVacio()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double TamanoTexto
    {
        get => (double)GetValue(TamanoTextoProperty);
        set => SetValue(TamanoTextoProperty, value);
    }

    public Color ColorTexto
    {
        get => (Color)GetValue(ColorTextoProperty)!;
        set => SetValue(ColorTextoProperty, value);
    }
}
