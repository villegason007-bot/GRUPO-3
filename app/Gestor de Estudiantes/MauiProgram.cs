
using Gestor_de_Estudiantes.Config;
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

            builder.Services.AddSingleton<IEstudianteService, EstudianteService>();
            builder.Services.AddTransient<EstudiantesViewModel>();
            builder.Services.AddTransient<EstudiantesPage>();

            // Comisiones
            builder.Services.AddSingleton<
                IComisionRepository,
                ComisionRepository>();

            builder.Services.AddSingleton<IComisionService, ComisionService>();
            // AHORA
            builder.Services.AddSingleton<ComisionesViewModel>();
            builder.Services.AddTransient<MateriasViewModel>();

            builder.Services.AddTransient<ComisionesPage>();
            builder.Services.AddTransient<ComisionDetallePage>();   // nueva
            builder.Services.AddTransient<EstudiantesPage>();       // ahora usa ComisionesViewModel
            builder.Services.AddTransient<MateriasPage>();
            builder.Services.AddTransient<ComisionesPage>();


            builder.Services.AddSingleton<IInscripcionMateriaRepository, InscripcionMateriaRepository>();
            builder.Services.AddSingleton<IInscripcionMateriaService, InscripcionMateriaService>();

            // Materias (N:M con comisiones, QA001)
            builder.Services.AddSingleton<
                IMateriaRepository,
                MateriaRepository>();

            builder.Services.AddTransient<IMateriaService, MateriaService>();
            builder.Services.AddTransient<ComisionMateriasViewModel>();

            // Contactos, ficha e historiales (H9-H13)
            builder.Services.AddSingleton<
                IContactoRepository,
                ContactoRepository>();

            builder.Services.AddTransient<IContactoService, ContactoService>();
            builder.Services.AddTransient<IFichaService, FichaService>();
            builder.Services.AddTransient<FichaEstudianteViewModel>();
            builder.Services.AddTransient<FichaEstudiantePage>();
            builder.Services.AddTransient<HistorialAsistenciaViewModel>();
            builder.Services.AddTransient<HistorialAsistenciaPage>();
            builder.Services.AddTransient<HistorialEntregasViewModel>();
            builder.Services.AddTransient<HistorialEntregasPage>();
            builder.Services.AddTransient<IHistorialAsistenciaService, HistorialAsistenciaService>();
            builder.Services.AddTransient<IHistorialEntregasService, HistorialEntregasService>();

            // Clases
            builder.Services.AddSingleton<
                IClaseRepository,
                ClaseRepository>();

            builder.Services.AddTransient<IClaseService, ClaseService>();

            // Trabajos
            builder.Services.AddSingleton<
                ITrabajoRepository,
                TrabajoRepository>();

            builder.Services.AddTransient<ITrabajoService, TrabajoService>();

            // Calendario (una sola fuente: ICalendarioService, SRP-2)
            builder.Services.AddSingleton<ICalendarioRepository, CalendarioRepository>();
            builder.Services.AddSingleton<ICalendarioService, CalendarioService>();
            builder.Services.AddTransient<CalendarioViewModel>();
            builder.Services.AddTransient<CalendarioPage>();

            // Asistencia y entregas
            builder.Services.AddSingleton<
                IAsistenciaRepository,
                AsistenciaRepository>();

            builder.Services.AddSingleton<
                IEntregaRepository,
                EntregaRepository>();

            builder.Services.AddTransient<IAsistenciaService, AsistenciaService>();
            builder.Services.AddTransient<IEntregaService, EntregaService>();

            builder.Services.AddTransient<AsistenciaEntregasViewModel>();
            builder.Services.AddTransient<AsistenciaEntregasPage>();

            // Riesgo
            builder.Services.AddSingleton<
                IRiesgoRepository,
                RiesgoRepository>();
            builder.Services.AddSingleton<
                IParametrosRiesgoRepository,
                ParametrosRiesgoRepository>();

            builder.Services.AddSingleton<ICalculadoraDeRiesgo, CalculadoraDeRiesgo>();
            builder.Services.AddSingleton<IRiesgoService, RiesgoService>();
            builder.Services.AddSingleton<IParametrosRiesgoService, ParametrosRiesgoService>();

            builder.Services.AddTransient<RiesgoViewModel>();
            builder.Services.AddTransient<RiesgoPage>();

            builder.Services.AddTransient<ConfiguracionRiesgoViewModel>();
            builder.Services.AddTransient<ConfiguracionRiesgoPage>();

            // Crear la base (esquema + semilla) al iniciar la app: en una
            // máquina recién clonada no hay base y sin esto la primera
            // pantalla falla con «SQLite Error 14» (la consola no corre en
            // el celular).
            BaseDeDatos.Inicializar();

            return builder.Build();
        }
    }
}
