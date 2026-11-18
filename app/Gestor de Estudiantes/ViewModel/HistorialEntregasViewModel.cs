using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;
using Microsoft.Maui.Graphics;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Historial de entregas e indicador de riesgo individual del
    /// estudiante (H13): trabajos de su comisión con el estado de
    /// entrega y el bloque de riesgo calculado por su legajo.
    /// </summary>
    public class HistorialEntregasViewModel : INotifyPropertyChanged
    {
        private readonly IEstudianteService _estudiantes;
        private readonly IHistorialEntregasService _historial;
        private readonly IRiesgoService _riesgo;

        private string _nombreCompleto = string.Empty;
        private string _subtitulo = string.Empty;
        private string _mensaje = string.Empty;
        private bool _mostrarMensaje;
        private string _legajo = string.Empty;

        private RiesgoIndividualViewModel _riesgoIndividual =
            RiesgoIndividualViewModel.Vacio();

        public HistorialEntregasViewModel(
            IEstudianteService estudiantes,
            IHistorialEntregasService historial,
            IRiesgoService riesgo)
        {
            _estudiantes = estudiantes;
            _historial = historial;
            _riesgo = riesgo;
        }

        /// <summary>Trabajos de la comisión con el estado del estudiante, del más próximo al más lejano.</summary>
        public ObservableCollection<FilaEntregaHistorial> Filas { get; } = new();

        /// <summary>Bloque de riesgo individual del estudiante (H13).</summary>
        public RiesgoIndividualViewModel RiesgoIndividual
        {
            get => _riesgoIndividual;
            private set => Notificar(ref _riesgoIndividual, value);
        }

        /// <summary>Nombre completo del estudiante (cabecera).</summary>
        public string NombreCompleto
        {
            get => _nombreCompleto;
            private set => Notificar(ref _nombreCompleto, value);
        }

        /// <summary>Legajo y comisión (subtítulo de cabecera).</summary>
        public string Subtitulo
        {
            get => _subtitulo;
            private set => Notificar(ref _subtitulo, value);
        }

        /// <summary>Mensaje de estado o de error para la página.</summary>
        public string Mensaje
        {
            get => _mensaje;
            private set => Notificar(ref _mensaje, value);
        }

        /// <summary>true si hay mensaje para mostrar.</summary>
        public bool MostrarMensaje
        {
            get => _mostrarMensaje;
            private set => Notificar(ref _mostrarMensaje, value);
        }

        /// <summary>
        /// Carga el historial de entregas (H13) y el indicador de riesgo
        /// individual del estudiante.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante a consultar.</param>
        public void CargarHistorial(string legajo)
        {
            try
            {
                var estudiante = _estudiantes.BuscarEstudiante(legajo)
                    ?? throw new InvalidOperationException(
                        $"No existe el estudiante {legajo}.");

                _legajo = estudiante.Legajo;
                NombreCompleto =
                    $"{estudiante.Nombre} {estudiante.Apellido}".Trim();
                Subtitulo =
                    $"Legajo {estudiante.Legajo}  ·  Comisión {estudiante.CodigoComision}";

                RiesgoIndividual =
                    RiesgoIndividualViewModel.Desde(
                        _riesgo.CalcularRiesgoDe(_legajo));

                Filas.Clear();
                foreach (var registro in _historial.HistorialEntregas(_legajo))
                    Filas.Add(new FilaEntregaHistorial(registro));

                MostrarMensaje = Filas.Count == 0;
                Mensaje = Filas.Count == 0
                    ? "Todavía no hay trabajos cargados en la comisión."
                    : $"Trabajos de la comisión: {Filas.Count}.";
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Filas.Clear();
                RiesgoIndividual = RiesgoIndividualViewModel.Vacio();
                Mensaje = ex.Message;
                MostrarMensaje = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notificar<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return;

            campo = valor;
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre));
        }
    }

    /// <summary>Fila del historial de entregas (H13) con textos para la vista.</summary>
    public class FilaEntregaHistorial
    {
        public FilaEntregaHistorial(RegistroDeEntrega registro)
        {
            Titulo = registro.Titulo;
            FechaVencimientoTexto =
                $"Vence {registro.FechaDeEntrega:dd/MM/yyyy}";

            if (registro.Entregado)
            {
                EstadoTexto = registro.Fecha is null
                    ? "Entregado"
                    : $"Entregado {registro.Fecha:dd/MM/yyyy}";
                BadgeColor = "#14532D";
                BadgeTextColor = "#DCFCE7";
            }
            else
            {
                EstadoTexto = "Sin entregar";
                BadgeColor = registro.FechaDeEntrega < DateTime.Today
                    ? "#7F1D1D"
                    : "#334155";
                BadgeTextColor = registro.FechaDeEntrega < DateTime.Today
                    ? "#FEE2E2"
                    : "#E2E8F0";
            }
        }

        /// <summary>Título del trabajo.</summary>
        public string Titulo { get; }

        /// <summary>Fecha límite del trabajo.</summary>
        public string FechaVencimientoTexto { get; }

        /// <summary>Estado mostrado («Entregado dd/MM/aaaa» o «Sin entregar»).</summary>
        public string EstadoTexto { get; }

        /// <summary>Fondo del badge de estado.</summary>
        public string BadgeColor { get; }

        /// <summary>Texto del badge de estado.</summary>
        public string BadgeTextColor { get; }
    }

    /// <summary>
    /// Bloque de riesgo individual (H13): estado, motivo y conteos del
    /// estudiante, con la semántica «Sin información» cuando no hay datos.
    /// </summary>
    public class RiesgoIndividualViewModel
    {
        private RiesgoIndividualViewModel(
            string estado,
            string motivo,
            string faltas,
            string trabajos,
            Color badgeColor,
            Color badgeTextColor)
        {
            Estado = estado;
            Motivo = motivo;
            Faltas = faltas;
            Trabajos = trabajos;
            BadgeColor = badgeColor;
            BadgeTextColor = badgeTextColor;
        }

        /// <summary>Estado a mostrar («En riesgo», «Sin riesgo» o «Sin información»).</summary>
        public string Estado { get; }

        /// <summary>Motivo que justifica el estado.</summary>
        public string Motivo { get; }

        /// <summary>Conteo de faltas sobre el máximo («—» sin información).</summary>
        public string Faltas { get; }

        /// <summary>Conteo de no entregados sobre el máximo («—» sin información).</summary>
        public string Trabajos { get; }

        /// <summary>Fondo del badge de estado.</summary>
        public Color BadgeColor { get; }

        /// <summary>Texto del badge de estado.</summary>
        public Color BadgeTextColor { get; }

        /// <summary>Bloque vacío, para antes de cargar o ante un error.</summary>
        public static RiesgoIndividualViewModel Vacio()
            => new(
                "—",
                "Sin datos para evaluar.",
                "—",
                "—",
                Color.FromArgb("#334155"),
                Color.FromArgb("#E2E8F0"));

        /// <summary>Traduce el resultado del cálculo de riesgo a textos de vista.</summary>
        public static RiesgoIndividualViewModel Desde(ResultadoRiesgo resultado)
        {
            if (resultado.SinInformacion)
            {
                return new(
                    "Sin información",
                    resultado.MotivoRiesgo,
                    "—",
                    "—",
                    Color.FromArgb("#334155"),
                    Color.FromArgb("#E2E8F0"));
            }

            var enRiesgo = resultado.EnRiesgo;

            return new(
                resultado.Estado,
                resultado.MotivoRiesgo,
                $"{resultado.Faltas} / {resultado.MaxFaltasPermitidas}",
                $"{resultado.NoEntregados} / {resultado.MaxTrabajosNoEntregadosPermitidos}",
                enRiesgo ? Color.FromArgb("#7F1D1D") : Color.FromArgb("#14532D"),
                enRiesgo ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#DCFCE7"));
        }
    }
}
