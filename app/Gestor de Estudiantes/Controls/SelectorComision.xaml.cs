using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.Controls;

/// <summary>
/// Tarjeta "Comisión activa": encabezado, picker de comisión (bindeado a
/// Comisiones/ComisionSeleccionada del ViewModel de la página) y botón de
/// refresco (RecargarCommand). Estaba duplicada en AsistenciaEntregasPage
/// y CalendarioPage.
/// </summary>
public partial class SelectorComision : ContentView
{
    public static readonly BindableProperty SubtituloProperty =
        BindableProperty.Create(nameof(Subtitulo), typeof(string), typeof(SelectorComision), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => ((SelectorComision)bindable).SubtituloLabel.Text = (string)newValue);

    public static readonly BindableProperty TituloPickerProperty =
        BindableProperty.Create(nameof(TituloPicker), typeof(string), typeof(SelectorComision),
            "Seleccionar comisión",
            propertyChanged: (bindable, oldValue, newValue) => ((SelectorComision)bindable).ComisionPicker.Title = (string)newValue);

    public static readonly BindableProperty TextoBotonProperty =
        BindableProperty.Create(nameof(TextoBoton), typeof(string), typeof(SelectorComision), "Actualizar",
            propertyChanged: (bindable, oldValue, newValue) => ((SelectorComision)bindable).ActualizarButton.Text = (string)newValue);

    public SelectorComision()
    {
        InitializeComponent();

        // Aplicar los valores por defecto (propertyChanged no se dispara hasta que cambien).
        SubtituloLabel.Text = Subtitulo;
        ComisionPicker.Title = TituloPicker;
        ActualizarButton.Text = TextoBoton;
    }

    /// <summary>Frase de ayuda debajo del título de la tarjeta.</summary>
    public string Subtitulo
    {
        get => (string)GetValue(SubtituloProperty);
        set => SetValue(SubtituloProperty, value);
    }

    /// <summary>Texto del placeholder del picker de comisiones.</summary>
    public string TituloPicker
    {
        get => (string)GetValue(TituloPickerProperty);
        set => SetValue(TituloPickerProperty, value);
    }

    /// <summary>Etiqueta del botón de refresco.</summary>
    public string TextoBoton
    {
        get => (string)GetValue(TextoBotonProperty);
        set => SetValue(TextoBotonProperty, value);
    }
}
