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
        BindableProperty.Create(nameof(Subtitulo), typeof(string), typeof(SelectorComision), string.Empty);

    public static readonly BindableProperty TituloPickerProperty =
        BindableProperty.Create(nameof(TituloPicker), typeof(string), typeof(SelectorComision),
            "Seleccionar comisión");

    public static readonly BindableProperty TextoBotonProperty =
        BindableProperty.Create(nameof(TextoBoton), typeof(string), typeof(SelectorComision), "Actualizar");

    public SelectorComision()
    {
        InitializeComponent();
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
