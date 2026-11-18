using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Gestor_de_Estudiantes.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping("BarraOscura", (handler, view) =>
            {
                var hwnd = WindowNative.GetWindowHandle(handler.PlatformView);
                var id = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(id);
                if (appWindow is null) return;

                var barra = appWindow.TitleBar;
                var fondo = Windows.UI.Color.FromArgb(255, 15, 23, 42);
                var texto = Windows.UI.Color.FromArgb(255, 248, 250, 252);
                var hover = Windows.UI.Color.FromArgb(255, 30, 58, 95);
                var apagado = Windows.UI.Color.FromArgb(255, 148, 163, 184);

                barra.BackgroundColor = fondo;
                barra.InactiveBackgroundColor = fondo;
                barra.ForegroundColor = texto;
                barra.InactiveForegroundColor = apagado;
                barra.ButtonBackgroundColor = fondo;
                barra.ButtonInactiveBackgroundColor = fondo;
                barra.ButtonForegroundColor = texto;
                barra.ButtonInactiveForegroundColor = apagado;
                barra.ButtonHoverBackgroundColor = hover;
                barra.ButtonHoverForegroundColor = texto;
            });
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}