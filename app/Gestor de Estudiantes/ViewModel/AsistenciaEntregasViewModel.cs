
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class AsistenciaEntregasViewModel : INotifyPropertyChanged
    {
        private readonly IComisionService _comisionService;
        private readonly IClaseService _claseService;
        private readonly ITrabajoService _trabajoService;
        private readonly IEstudianteService _estudianteService;
        private readonly IAsistenciaService _asistenciaService;
        private readonly IEntregaService _entregaService;

        private Comision? _comisionSeleccionada;
        private Clase? _claseSeleccionada;
        private Trabajo? _trabajoSeleccionado;
        private string _mensaje = string.Empty;
        private bool _estaCargando;

        public ObservableCollection<Comision> Comisiones { get; } = new();
        public ObservableCollection<Clase> Clases { get; } = new();
        public ObservableCollection<Trabajo> Trabajos { get; } = new();
        public ObservableCollection<FilaAsistencia> Asistencias { get; } = new();
        public ObservableCollection<FilaEntrega> Entregas { get; } = new();

        public Comision? ComisionSeleccionada
        {
            get => _comisionSeleccionada;
            set
            {
                if (SetProperty(ref _comisionSeleccionada, value))
                    CargarDatosComision();
            }
        }

        public Clase? ClaseSeleccionada
        {
            get => _claseSeleccionada;
            set
            {
                if (SetProperty(ref _claseSeleccionada, value))
                    CargarAsistencias();
            }
        }

        public Trabajo? TrabajoSeleccionado
        {
            get => _trabajoSeleccionado;
            set
            {
                if (SetProperty(ref _trabajoSeleccionado, value))
                    CargarEntregas();
            }
        }

        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public bool EstaCargando
        {
            get => _estaCargando;
            set => SetProperty(ref _estaCargando, value);
        }

        public ICommand GuardarAsistenciasCommand { get; }
        public ICommand GuardarEntregasCommand { get; }
        public ICommand RecargarCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AsistenciaEntregasViewModel(
            IComisionService comisionService,
            IClaseService claseService,
            ITrabajoService trabajoService,
            IEstudianteService estudianteService,
            IAsistenciaService asistenciaService,
            IEntregaService entregaService)
        {
            _comisionService = comisionService;
            _claseService = claseService;
            _trabajoService = trabajoService;
            _estudianteService = estudianteService;
            _asistenciaService = asistenciaService;
            _entregaService = entregaService;

            GuardarAsistenciasCommand = new Command(GuardarAsistencias);
            GuardarEntregasCommand = new Command(GuardarEntregas);
            RecargarCommand = new Command(CargarDatosComision);

            CargarComisiones();
        }

        private void CargarComisiones()
        {
            try
            {
                Comisiones.Clear();

                foreach (var comision in _comisionService.ListarComisiones())
                    Comisiones.Add(comision);

                ComisionSeleccionada = Comisiones.FirstOrDefault();

                if (ComisionSeleccionada == null)
                    Mensaje = "No hay comisiones registradas.";
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar las comisiones: {ex.Message}";
            }
        }

        private void CargarDatosComision()
        {
            Clases.Clear();
            Trabajos.Clear();
            Asistencias.Clear();
            Entregas.Clear();

            if (ComisionSeleccionada == null)
                return;

            try
            {
                var codigo = ComisionSeleccionada.Codigo;

                foreach (var clase in _claseService.ListarClases(codigo))
                    Clases.Add(clase);

                foreach (var trabajo in _trabajoService.ListarTrabajos(codigo))
                    Trabajos.Add(trabajo);

                ClaseSeleccionada = Clases.FirstOrDefault();
                TrabajoSeleccionado = Trabajos.FirstOrDefault();

                if (Clases.Count == 0 && Trabajos.Count == 0)
                    Mensaje = "Esta comisión todavía no tiene clases ni trabajos.";
                else
                    Mensaje = string.Empty;
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar los datos: {ex.Message}";
            }
        }

        private void CargarAsistencias()
        {
            Asistencias.Clear();

            if (ComisionSeleccionada == null || ClaseSeleccionada == null)
                return;

            try
            {
                var estudiantes = _estudianteService.ListarEstudiantes(
                    ComisionSeleccionada.Codigo);

                var existentes = _asistenciaService.ListarPorClase(
                    ClaseSeleccionada.Id);

                foreach (var estudiante in estudiantes)
                {
                    var asistencia = existentes.FirstOrDefault(a =>
                        a.EstudianteId == estudiante.Id);

                    Asistencias.Add(new FilaAsistencia
                    {
                        Estudiante = estudiante,
                        Presente = asistencia?.EstaPresente ?? false,
                        TieneRegistro = asistencia != null
                    });
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar las asistencias: {ex.Message}";
            }
        }

        private void CargarEntregas()
        {
            Entregas.Clear();

            if (ComisionSeleccionada == null || TrabajoSeleccionado == null)
                return;

            try
            {
                var estudiantes = _estudianteService.ListarEstudiantes(
                    ComisionSeleccionada.Codigo);

                var existentes = _entregaService.ListarPorTrabajo(
                    TrabajoSeleccionado.Id);

                foreach (var estudiante in estudiantes)
                {
                    var entrega = existentes.FirstOrDefault(e =>
                        e.EstudianteId == estudiante.Id);

                    Entregas.Add(new FilaEntrega
                    {
                        Estudiante = estudiante,
                        Entregado = entrega?.Entregado ?? false,
                        Fecha = entrega?.Fecha ?? DateTime.Today,
                        TieneRegistro = entrega != null
                    });
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar las entregas: {ex.Message}";
            }
        }

        private void GuardarAsistencias()
        {
            if (ClaseSeleccionada == null)
            {
                Mensaje = "Seleccioná una clase.";
                return;
            }

            if (ClaseSeleccionada.Estado != EstadoClase.Habil)
            {
                Mensaje = "Esta clase no está habilitada para registrar asistencia.";
                return;
            }

            try
            {
                foreach (var fila in Asistencias)
                {
                    _asistenciaService.GuardarAsistencia(
                        ClaseSeleccionada.Id,
                        fila.Estudiante.Id,
                        fila.Presente);
                }

                Mensaje = "Las asistencias se guardaron correctamente.";
                CargarAsistencias();
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al guardar las asistencias: {ex.Message}";
            }
        }

        private void GuardarEntregas()
        {
            if (TrabajoSeleccionado == null)
            {
                Mensaje = "Seleccioná un trabajo.";
                return;
            }

            try
            {
                foreach (var fila in Entregas)
                {
                    _entregaService.GuardarEntrega(
                        TrabajoSeleccionado.Id,
                        fila.Estudiante.Id,
                        fila.Entregado,
                        fila.Entregado ? fila.Fecha.Date : null);
                }

                Mensaje = "Las entregas se guardaron correctamente.";
                CargarEntregas();
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al guardar las entregas: {ex.Message}";
            }
        }

        private bool SetProperty<T>(
            ref T campo,
            T valor,
            [CallerMemberName] string? nombre = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return false;

            campo = valor;
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre));

            return true;
        }
    }

    public class FilaAsistencia : INotifyPropertyChanged
    {
        private bool _presente;

        public Estudiante Estudiante { get; set; } = new();

        public bool TieneRegistro { get; set; }

        public bool Presente
        {
            get => _presente;
            set
            {
                if (_presente == value)
                    return;

                _presente = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Presente)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class FilaEntrega : INotifyPropertyChanged
    {
        private bool _entregado;
        private DateTime _fecha = DateTime.Today;

        public Estudiante Estudiante { get; set; } = new();

        public bool TieneRegistro { get; set; }

        public bool Entregado
        {
            get => _entregado;
            set
            {
                if (_entregado == value)
                    return;

                _entregado = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Entregado)));
            }
        }

        public DateTime Fecha
        {
            get => _fecha;
            set
            {
                if (_fecha == value)
                    return;

                _fecha = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Fecha)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
