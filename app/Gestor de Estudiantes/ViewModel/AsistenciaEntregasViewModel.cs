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
        private readonly IMateriaService _materiaService;

        private Comision? _comisionSeleccionada;
        private Clase? _claseSeleccionada;
        private Trabajo? _trabajoSeleccionado;
        private Materia? _materiaSeleccionada;
        private string _mensaje = string.Empty;
        private bool _estaCargando;
        private bool _cargandoDatosComision;

        public ObservableCollection<Comision> Comisiones { get; } = new();
        public ObservableCollection<Clase> Clases { get; } = new();
        public ObservableCollection<Trabajo> Trabajos { get; } = new();
        public ObservableCollection<Materia> Materias { get; } = new();
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

        public Materia? MateriaSeleccionada
        {
            get => _materiaSeleccionada;
            set
            {
                if (SetProperty(ref _materiaSeleccionada, value) &&
                    !_cargandoDatosComision)
                    CargarDatosMateria();
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
            IEntregaService entregaService,
            IMateriaService materiaService)
        {
            _comisionService = comisionService;
            _claseService = claseService;
            _trabajoService = trabajoService;
            _estudianteService = estudianteService;
            _asistenciaService = asistenciaService;
            _entregaService = entregaService;
            _materiaService = materiaService;

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
            if (_cargandoDatosComision)
                return;

            _cargandoDatosComision = true;

            try
            {
                Clases.Clear();
                Trabajos.Clear();
                Asistencias.Clear();
                Entregas.Clear();

                if (ComisionSeleccionada == null)
                    return;

                var codigo = ComisionSeleccionada.Codigo;
                var materiaIdSeleccionada = _materiaSeleccionada?.Id;

                Materias.Clear();
                foreach (var materia in _materiaService.ListarMateriasDeComision(codigo))
                    Materias.Add(materia);

                // No usar el setter aquí: el setter vuelve a llamar a
                // CargarDatosComision() y provoca una recursión infinita.
                var materiaSeleccionada = Materias.FirstOrDefault(m =>
                    m.Id == materiaIdSeleccionada)
                    ?? Materias.FirstOrDefault();

                if (!ReferenceEquals(_materiaSeleccionada, materiaSeleccionada))
                {
                    _materiaSeleccionada = materiaSeleccionada;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(MateriaSeleccionada)));
                }

                CargarDatosMateria();
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar los datos: {ex.Message}";
            }
            finally
            {
                _cargandoDatosComision = false;
            }
        }

        private void CargarDatosMateria()
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

                var clases = _claseService.ListarClases(codigo)
                    .Where(c => MateriaSeleccionada == null ||
                                c.MateriaId == MateriaSeleccionada.Id);

                foreach (var clase in clases)
                    Clases.Add(clase);

                var trabajos = _trabajoService.ListarTrabajos(codigo)
                    .Where(t => MateriaSeleccionada == null ||
                                t.MateriaId == MateriaSeleccionada.Id);

                foreach (var trabajo in trabajos)
                    Trabajos.Add(trabajo);

                ClaseSeleccionada = Clases.FirstOrDefault();
                TrabajoSeleccionado = Trabajos.FirstOrDefault();

                Mensaje = Clases.Count == 0 && Trabajos.Count == 0
                    ? "Esta comisión todavía no tiene clases ni trabajos."
                    : string.Empty;
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

                    // Sin registro la fila nace Pendiente: el tutor marca
                    // Presente/Ausente o la deja Sin marcar.
                    Asistencias.Add(new FilaAsistencia
                    {
                        Estudiante = estudiante,
                        Condicion = asistencia?.Condicion ?? Condicion.Pendiente,
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
                        fila.Condicion);
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
        private Condicion _condicion = Condicion.Pendiente;

        public Estudiante Estudiante { get; set; } = new();

        public bool TieneRegistro { get; set; }

        /// <summary>
        /// Condición elegida para la fila; arranca en Pendiente (sin
        /// marcar) y la cambian los comandos de los tres botones.
        /// </summary>
        public Condicion Condicion
        {
            get => _condicion;
            set
            {
                if (_condicion == value)
                    return;

                _condicion = value;
                Notificar(nameof(Condicion));
                Notificar(nameof(EsPresente));
                Notificar(nameof(EsAusente));
                Notificar(nameof(EsPendiente));
                Notificar(nameof(TextoCondicion));
            }
        }

        /// <summary>true si la fila quedó marcada como presente.</summary>
        public bool EsPresente => Condicion == Condicion.Presente;

        /// <summary>true si la fila quedó marcada como ausente.</summary>
        public bool EsAusente => Condicion == Condicion.Ausente;

        /// <summary>true si la fila sigue sin marcar (Pendiente).</summary>
        public bool EsPendiente => Condicion == Condicion.Pendiente;

        /// <summary>Texto a mostrar de la condición («Sin marcar» para Pendiente).</summary>
        public string TextoCondicion => Condicion switch
        {
            Condicion.Presente => "Presente",
            Condicion.Ausente => "Ausente",
            _ => "Sin marcar",
        };

        public ICommand MarcarPresenteCommand { get; }
        public ICommand MarcarAusenteCommand { get; }
        public ICommand MarcarPendienteCommand { get; }

        public FilaAsistencia()
        {
            MarcarPresenteCommand = new Command(() => Condicion = Condicion.Presente);
            MarcarAusenteCommand = new Command(() => Condicion = Condicion.Ausente);
            MarcarPendienteCommand = new Command(() => Condicion = Condicion.Pendiente);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notificar(string nombre)
            => PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre));
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
