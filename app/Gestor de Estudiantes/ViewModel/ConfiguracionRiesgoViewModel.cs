
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestor_de_Estudiantes.Services;
using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class ConfiguracionRiesgoViewModel : INotifyPropertyChanged
    {
        private readonly IParametrosRiesgoService _riesgoService;

        private string _maxFaltasPermitidasTexto = string.Empty;
        private string _maxTrabajosNoEntregadosPermitidosTexto = string.Empty;
        private string _mensaje = string.Empty;
        private Color _mensajeColor = Color.FromArgb("#94A3B8");

        public string MaxFaltasPermitidasTexto
        {
            get => _maxFaltasPermitidasTexto;
            set
            {
                if (_maxFaltasPermitidasTexto == value)
                    return;

                _maxFaltasPermitidasTexto = value;
                OnPropertyChanged();
            }
        }

        public string MaxTrabajosNoEntregadosPermitidosTexto
        {
            get => _maxTrabajosNoEntregadosPermitidosTexto;
            set
            {
                if (_maxTrabajosNoEntregadosPermitidosTexto == value)
                    return;

                _maxTrabajosNoEntregadosPermitidosTexto = value;
                OnPropertyChanged();
            }
        }

        public string Mensaje
        {
            get => _mensaje;
            private set
            {
                if (_mensaje == value)
                    return;

                _mensaje = value;
                OnPropertyChanged();
            }
        }

        public Color MensajeColor
        {
            get => _mensajeColor;
            private set
            {
                if (_mensajeColor == value)
                    return;

                _mensajeColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }

        public ConfiguracionRiesgoViewModel(IParametrosRiesgoService riesgoService)
        {
            _riesgoService = riesgoService;

            GuardarCommand = new Command(Guardar);

            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            MaxFaltasPermitidasTexto =
                _riesgoService.ObtenerMaxFaltasPermitidas().ToString();

            MaxTrabajosNoEntregadosPermitidosTexto =
                _riesgoService.ObtenerMaxTrabajosNoEntregadosPermitidos()
                    .ToString();
        }

        private void Guardar()
        {
            if (!int.TryParse(MaxFaltasPermitidasTexto, out var maxFaltas)
                || maxFaltas < 0)
            {
                MostrarError(
                    "Ingresá un máximo de faltas válido, igual o mayor que cero.");
                return;
            }

            if (!int.TryParse(
                    MaxTrabajosNoEntregadosPermitidosTexto,
                    out var maxTrabajos)
                || maxTrabajos < 0)
            {
                MostrarError(
                    "Ingresá un máximo de trabajos válido, igual o mayor que cero.");
                return;
            }

            try
            {
                _riesgoService.GuardarMaxFaltasPermitidas(maxFaltas);

                _riesgoService.GuardarMaxTrabajosNoEntregadosPermitidos(
                    maxTrabajos);

                Mensaje = "Configuración guardada correctamente.";
                MensajeColor = Color.FromArgb("#86EFAC");
            }
            catch (Exception ex)
            {
                MostrarError($"No se pudo guardar la configuración: {ex.Message}");
            }
        }

        private void MostrarError(string mensaje)
        {
            Mensaje = mensaje;
            MensajeColor = Color.FromArgb("#FCA5A5");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
