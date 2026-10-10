using Microsoft.Extensions.Logging;
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;
using Gestor_de_Estudiantes.ViewModel;
using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Services.AddSingleton<IEstudianteRepository, EstudianteRepository>();
            builder.Services.AddSingleton<EstudianteService>();
            builder.Services.AddTransient<EstudiantesViewModel>();
            builder.Services.AddTransient<EstudiantesPage>();
            builder.Services.AddSingleton<IComisionRepository, ComisionRepository>();
            builder.Services.AddSingleton<ComisionService>();
            builder.Services.AddTransient<ComisionesViewModel>();
            builder.Services.AddTransient<ComisionesPage>();
#endif

            return builder.Build();
        }
    }
}
