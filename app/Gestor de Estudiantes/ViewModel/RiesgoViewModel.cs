
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;
using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class RiesgoViewModel : INotifyPropertyChanged
    {
        private readonly IComisionService _comisionService;
        private readonly IRiesgoService _riesgoService;

        private Comision? _comisionSeleccionada;
        private string _mensaje = string.Empty;
        private bool _mostrarMensaje;

        public ObservableCollection<Comision> Comisiones { get; } = new();

        public ObservableCollection<RiesgoFilaViewModel> Estudiantes { get; } = new();

        public Comision? ComisionSeleccionada
        {
            get => _comisionSeleccionada;
            set
            {
                if (_comisionSeleccionada == value)
                    return;

                _comisionSeleccionada = value;
                OnPropertyChanged();
                CargarRiesgos();
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

        public bool MostrarMensaje
        {
            get => _mostrarMensaje;
            private set
            {
                if (_mostrarMensaje == value)
                    return;

                _mostrarMensaje = value;
                OnPropertyChanged();
            }
        }

        public RiesgoViewModel(
            IComisionService comisionService,
            IRiesgoService riesgoService)
        {
            _comisionService = comisionService;
            _riesgoService = riesgoService;

            CargarComisiones();
        }

        private void CargarComisiones()
        {
            try
            {
                Comisiones.Clear();

                foreach (var comision in _comisionService.ListarComisiones())
                    Comisiones.Add(comision);

                if (Comisiones.Count > 0)
                {
                    ComisionSeleccionada = Comisiones[0];
                }
                else
                {
                    Estudiantes.Clear();
                    MostrarMensaje = true;
                    Mensaje = "No hay comisiones registradas.";
                }
            }
            catch (Exception ex)
            {
                Estudiantes.Clear();
                MostrarMensaje = true;
                Mensaje = $"No se pudieron cargar las comisiones: {ex.Message}";
            }
        }

        private void CargarRiesgos()
        {
            Estudiantes.Clear();

            if (ComisionSeleccionada is null)
            {
                MostrarMensaje = true;
                Mensaje = "Seleccioná una comisión para consultar sus estudiantes.";
                return;
            }

            try
            {
                var resultados = _riesgoService
                    .CalcularRiesgos(ComisionSeleccionada.Codigo);

                var ordenados = resultados
                    .OrderBy(r => r.EnRiesgo ? 0 : r.SinInformacion ? 1 : 2)
                    .ThenBy(r => r.Nombre)
                    .ToList();

                foreach (var resultado in ordenados)
                    Estudiantes.Add(new RiesgoFilaViewModel(resultado));

                MostrarMensaje = resultados.Count == 0;
                Mensaje = resultados.Count == 0
                    ? "No hay estudiantes registrados en esta comisión."
                    : $"Estudiantes evaluados: {resultados.Count}.";
            }
            catch (Exception ex)
            {
                MostrarMensaje = true;
                Mensaje = $"No se pudo calcular el riesgo: {ex.Message}";
            }
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

    public class RiesgoFilaViewModel
    {
        public string Nombre { get; }
        public string TrabajosResumen { get; }
        public string FaltasResumen { get; }
        public string Motivo { get; }
        public string Estado { get; }
        public Color BadgeColor { get; }
        public Color BadgeTextColor { get; }

        public RiesgoFilaViewModel(ResultadoRiesgo resultado)
        {
            Nombre = resultado.Nombre;
            Estado = resultado.Estado;
            Motivo = resultado.MotivoRiesgo;

            if (resultado.SinInformacion)
            {
                TrabajosResumen = "—";
                FaltasResumen = "—";
                BadgeColor = Color.FromArgb("#334155");
                BadgeTextColor = Color.FromArgb("#E2E8F0");
            }
            else
            {
                TrabajosResumen =
                    $"{resultado.NoEntregados} / {resultado.MaxTrabajosNoEntregadosPermitidos}";

                FaltasResumen =
                    $"{resultado.Faltas} / {resultado.MaxFaltasPermitidas}";

                if (resultado.EnRiesgo)
                {
                    BadgeColor = Color.FromArgb("#7F1D1D");
                    BadgeTextColor = Color.FromArgb("#FEE2E2");
                }
                else
                {
                    BadgeColor = Color.FromArgb("#14532D");
                    BadgeTextColor = Color.FromArgb("#DCFCE7");
                }
            }
        }
    }
}
