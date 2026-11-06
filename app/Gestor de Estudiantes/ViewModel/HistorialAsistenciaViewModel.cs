using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Historial de asistencia del estudiante (H12): fechas de las
    /// clases con registro y la condición de cada una (presente,
    /// ausente o sin marcar).
    /// </summary>
    public class HistorialAsistenciaViewModel : INotifyPropertyChanged
    {
        private readonly IEstudianteService _estudiantes;
        private readonly IHistorialAsistenciaService _historial;

        private string _nombreCompleto = string.Empty;
        private string _subtitulo = string.Empty;
        private string _mensaje = string.Empty;
        private bool _mostrarMensaje;
        private string _legajo = string.Empty;

        public HistorialAsistenciaViewModel(
            IEstudianteService estudiantes,
            IHistorialAsistenciaService historial)
        {
            _estudiantes = estudiantes;
            _historial = historial;
        }

        /// <summary>Clases con registro del estudiante, de más reciente a más antigua.</summary>
        public ObservableCollection<FilaAsistenciaHistorial> Filas { get; } = new();

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
        /// Carga el historial de asistencia del estudiante (H12) y su
        /// identificación para la cabecera.
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

                Filas.Clear();
                foreach (var registro in _historial.HistorialAsistencia(_legajo))
                    Filas.Add(new FilaAsistenciaHistorial(registro));

                MostrarMensaje = Filas.Count == 0;
                Mensaje = Filas.Count == 0
                    ? "Todavía no hay asistencias registradas."
                    : $"Clases con registro: {Filas.Count}.";
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Filas.Clear();
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

    /// <summary>Fila del historial de asistencia (H12) con textos y colores para la vista.</summary>
    public class FilaAsistenciaHistorial
    {
        public FilaAsistenciaHistorial(RegistroDeAsistencia registro)
        {
            FechaTexto = registro.Fecha.ToString("dd/MM/yyyy");

            if (registro.Condicion == Condicion.Presente)
            {
                CondicionTexto = "Presente";
                BadgeColor = "#14532D";
                BadgeTextColor = "#DCFCE7";
            }
            else if (registro.Condicion == Condicion.Ausente)
            {
                CondicionTexto = "Ausente";
                BadgeColor = "#7F1D1D";
                BadgeTextColor = "#FEE2E2";
            }
            else
            {
                CondicionTexto = "Sin marcar";
                BadgeColor = "#334155";
                BadgeTextColor = "#E2E8F0";
            }
        }

        /// <summary>Fecha de la clase (dd/MM/aaaa).</summary>
        public string FechaTexto { get; }

        /// <summary>Condición mostrada («Presente», «Ausente» o «Sin marcar»).</summary>
        public string CondicionTexto { get; }

        /// <summary>Fondo del badge de condición.</summary>
        public string BadgeColor { get; }

        /// <summary>Texto del badge de condición.</summary>
        public string BadgeTextColor { get; }
    }
}
