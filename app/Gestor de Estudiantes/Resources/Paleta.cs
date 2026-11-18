using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.Resources
{
    /// <summary>
    /// Paleta de colores de la app en C#.
    /// Espejo de Resources/Styles/Colors.xaml: si cambia un color, cambia en ambos lugares.
    /// Evita los Color.FromArgb("#...") repetidos en el code-behind.
    /// </summary>
    public static class Paleta
    {
        /// <summary>Color de acento (violeta). En XAML: {StaticResource Acento}</summary>
        public static readonly Color Acento = Color.FromArgb("#7667F5");

        /// <summary>Texto secundario (gris azulado). En XAML: {StaticResource TextoSecundario}</summary>
        public static readonly Color TextoSecundario = Color.FromArgb("#AEBBD0");

        // Colores de estado (semáforo de riesgo / asistencia)
        public static readonly Color ExitoFondo = Color.FromArgb("#DCFCE7");
        public static readonly Color ExitoTexto = Color.FromArgb("#14532D");
        public static readonly Color ExitoSuave = Color.FromArgb("#86EFAC");
        public static readonly Color PeligroFondo = Color.FromArgb("#FEE2E2");
        public static readonly Color PeligroTexto = Color.FromArgb("#7F1D1D");
        public static readonly Color PeligroSuave = Color.FromArgb("#FCA5A5");
        public static readonly Color NeutroTexto = Color.FromArgb("#94A3B8");
        public static readonly Color NeutroBorde = Color.FromArgb("#334155");
        public static readonly Color NeutroClaro = Color.FromArgb("#E2E8F0");
    }
}
