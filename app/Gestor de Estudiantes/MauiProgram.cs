
using Gestor_de_Estudiantes.Repositories;
using Gestor_de_Estudiantes.Services;
using Gestor_de_Estudiantes.ViewModel;
using Gestor_de_Estudiantes.Views;
using Microsoft.Extensions.Logging;

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
                    fonts.AddFont(
                        "OpenSans-Regular.ttf",
                        "OpenSansRegular");

                    fonts.AddFont(
                        "OpenSans-Semibold.ttf",
                        "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Estudiantes
            builder.Services.AddSingleton<
                IEstudianteRepository,
                EstudianteRepository>();

            builder.Services.AddSingleton<EstudianteService>();
            builder.Services.AddTransient<EstudiantesViewModel>();
            builder.Services.AddTransient<EstudiantesPage>();

            // Comisiones
            builder.Services.AddSingleton<
                IComisionRepository,
                ComisionRepository>();

            builder.Services.AddSingleton<ComisionService>();
            builder.Services.AddTransient<ComisionesViewModel>();
            builder.Services.AddTransient<ComisionesPage>();

            // Clases
            builder.Services.AddSingleton<
                IClaseRepository,
                ClaseRepository>();

            builder.Services.AddTransient<ClaseService>();

            // Trabajos
            builder.Services.AddSingleton<
                ITrabajoRepository,
                TrabajoRepository>();

            builder.Services.AddTransient<TrabajoService>();

            // Calendario
            builder.Services.AddTransient<CalendarioViewModel>();
            builder.Services.AddTransient<CalendarioPage>();

            // Asistencia y entregas
            builder.Services.AddSingleton<
                IAsistenciaRepository,
                AsistenciaRepository>();

            builder.Services.AddSingleton<
                IEntregaRepository,
                EntregaRepository>();

            builder.Services.AddTransient<AsistenciaService>();
            builder.Services.AddTransient<EntregaService>();

            builder.Services.AddTransient<AsistenciaEntregasViewModel>();
            builder.Services.AddTransient<AsistenciaEntregasPage>();

            // Riesgo
            builder.Services.AddSingleton<
                IRiesgoRepository,
                RiesgoRepository>();

            builder.Services.AddSingleton<RiesgoService>();

            builder.Services.AddTransient<RiesgoViewModel>();
            builder.Services.AddTransient<RiesgoPage>();

            builder.Services.AddTransient<ConfiguracionRiesgoViewModel>();
            builder.Services.AddTransient<ConfiguracionRiesgoPage>();

            return builder.Build();
        }
    }
}
