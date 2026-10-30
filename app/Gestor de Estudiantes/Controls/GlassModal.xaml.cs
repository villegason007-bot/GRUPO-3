using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.Controls;

/// <summary>
/// Modal con fondo oscuro, encabezado (ícono, título y subtítulo), un slot para
/// el formulario de la página, mensaje de error y botones Cancelar/Confirmar.
/// Centraliza la estructura que estaba duplicada en los modales de ComisionesPage.
/// </summary>
public partial class GlassModal : ContentView
{
    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(nameof(Titulo), typeof(string), typeof(GlassModal), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).TituloLabel.Text = (string)newValue);

    public static readonly BindableProperty SubtituloProperty =
        BindableProperty.Create(nameof(Subtitulo), typeof(string), typeof(GlassModal), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).SubtituloLabel.Text = (string)newValue);

    public static readonly BindableProperty TextoConfirmarProperty =
        BindableProperty.Create(nameof(TextoConfirmar), typeof(string), typeof(GlassModal), "Guardar",
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).ConfirmarButton.Text = (string)newValue);

    public static readonly BindableProperty FormularioProperty =
        BindableProperty.Create(nameof(Formulario), typeof(View), typeof(GlassModal),
            propertyChanged: (bindable, oldValue, newValue) =>
                ((GlassModal)bindable).SlotFormulario.Content = newValue as View);

    public static readonly BindableProperty IconoProperty =
        BindableProperty.Create(nameof(Icono), typeof(string), typeof(GlassModal), "＋",
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).IconoLabel.Text = (string)newValue);

    public static readonly BindableProperty IconoFondoProperty =
        BindableProperty.Create(nameof(IconoFondo), typeof(Color), typeof(GlassModal),
            Color.FromArgb("#302F2A75"),
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).IconoBorder.Background = (Color)newValue);

    public static readonly BindableProperty IconoBordeProperty =
        BindableProperty.Create(nameof(IconoBorde), typeof(Color), typeof(GlassModal),
            Color.FromArgb("#505B5CF6"),
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).IconoBorder.Stroke = (Color)newValue);

    public static readonly BindableProperty IconoColorProperty =
        BindableProperty.Create(nameof(IconoColor), typeof(Color), typeof(GlassModal),
            Color.FromArgb("#C7D2FE"),
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).IconoLabel.TextColor = (Color)newValue);

    public static readonly BindableProperty IconoTamanoProperty =
        BindableProperty.Create(nameof(IconoTamano), typeof(double), typeof(GlassModal), 27d,
            propertyChanged: (bindable, oldValue, newValue) => ((GlassModal)bindable).IconoLabel.FontSize = (double)newValue);

    public GlassModal()
    {
        InitializeComponent();

        // Aplicar los valores por defecto (propertyChanged no se dispara hasta que cambien).
        TituloLabel.Text = Titulo;
        SubtituloLabel.Text = Subtitulo;
        ConfirmarButton.Text = TextoConfirmar;
        IconoLabel.Text = Icono;
        IconoBorder.Background = IconoFondo;
        IconoBorder.Stroke = IconoBorde;
        IconoLabel.TextColor = IconoColor;
        IconoLabel.FontSize = IconoTamano;
    }

    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public string Subtitulo
    {
        get => (string)GetValue(SubtituloProperty);
        set => SetValue(SubtituloProperty, value);
    }

    public string TextoConfirmar
    {
        get => (string)GetValue(TextoConfirmarProperty);
        set => SetValue(TextoConfirmarProperty, value);
    }

    /// <summary>Contenido del formulario (campos), que se aloja en el slot del modal.</summary>
    public View Formulario
    {
        get => (View)GetValue(FormularioProperty);
        set => SetValue(FormularioProperty, value);
    }

    public string Icono
    {
        get => (string)GetValue(IconoProperty);
        set => SetValue(IconoProperty, value);
    }

    public Color IconoFondo
    {
        get => (Color)GetValue(IconoFondoProperty)!;
        set => SetValue(IconoFondoProperty, value);
    }

    public Color IconoBorde
    {
        get => (Color)GetValue(IconoBordeProperty)!;
        set => SetValue(IconoBordeProperty, value);
    }

    public Color IconoColor
    {
        get => (Color)GetValue(IconoColorProperty)!;
        set => SetValue(IconoColorProperty, value);
    }

    public double IconoTamano
    {
        get => (double)GetValue(IconoTamanoProperty);
        set => SetValue(IconoTamanoProperty, value);
    }

    /// <summary>Se dispara al tocar "Cancelar" (la página cierra el modal).</summary>
    public event EventHandler? CancelarClicked;

    /// <summary>Se dispara al tocar el botón de confirmación del modal.</summary>
    public event EventHandler? ConfirmarClicked;

    /// <summary>Muestra un mensaje de error debajo de los campos.</summary>
    public void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }

    /// <summary>Limpia y oculta el mensaje de error.</summary>
    public void LimpiarError()
    {
        ErrorLabel.Text = string.Empty;
        ErrorLabel.IsVisible = false;
    }

    private void CancelarInterno_Clicked(object? sender, EventArgs e) =>
        CancelarClicked?.Invoke(this, EventArgs.Empty);

    private void ConfirmarInterno_Clicked(object? sender, EventArgs e) =>
        ConfirmarClicked?.Invoke(this, EventArgs.Empty);
}
