using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class CalendarioViewModel : INotifyPropertyChanged
    {
        private readonly IComisionService _comisionService;
        private readonly IClaseService _claseService;
        private readonly ITrabajoService _trabajoService;
        private readonly ICalendarioService _calendarioService;
        private readonly IMateriaService _materiaService;
        private readonly IEstudianteService _estudianteService;
        private readonly IAsistenciaService _asistenciaService;

        private Comision? _comisionSeleccionada;
        private DateTime _fechaClase = DateTime.Today;
        private DateTime _fechaEntrega = DateTime.Today;
        private string _tituloTrabajo = string.Empty;
        private string _mensaje = string.Empty;
        private bool _estaCargando;

        public ObservableCollection<Comision> Comisiones { get; } = new();

        public ObservableCollection<Materia> Materias { get; } = new();

        public ObservableCollection<ElementoCalendario> Eventos { get; } = new();

        public ObservableCollection<ClaseCalendarioItem> Clases { get; } = new();

        public ObservableCollection<AsistenciaCalendarioItem> Asistencias { get; } = new();

        public Comision? ComisionSeleccionada
        {
            get => _comisionSeleccionada;
            set
            {
                if (SetProperty(ref _comisionSeleccionada, value))
                {
                    CargarCalendario();
                }
            }
        }

        private Materia? _materiaSeleccionada;

        public Materia? MateriaSeleccionada
        {
            get => _materiaSeleccionada;
            set => SetProperty(ref _materiaSeleccionada, value);
        }

        public DateTime FechaClase
        {
            get => _fechaClase;
            set => SetProperty(ref _fechaClase, value);
        }

        public DateTime FechaEntrega
        {
            get => _fechaEntrega;
            set => SetProperty(ref _fechaEntrega, value);
        }

        public string TituloTrabajo
        {
            get => _tituloTrabajo;
            set => SetProperty(ref _tituloTrabajo, value);
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

        public bool SinEventos => Eventos.Count == 0;

        public ICommand RecargarCommand { get; }
        public ICommand RegistrarClaseCommand { get; }
        public ICommand RegistrarTrabajoCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CalendarioViewModel(
            IComisionService comisionService,
            IClaseService claseService,
            ITrabajoService trabajoService,
            ICalendarioService calendarioService,
            IMateriaService materiaService,
            IEstudianteService estudianteService,
            IAsistenciaService asistenciaService)
        {
            _comisionService = comisionService;
            _claseService = claseService;
            _trabajoService = trabajoService;
            _calendarioService = calendarioService;
            _materiaService = materiaService;
            _estudianteService = estudianteService;
            _asistenciaService = asistenciaService;

            RecargarCommand = new Command(CargarCalendario);
            RegistrarClaseCommand = new Command(RegistrarClase);
            RegistrarTrabajoCommand = new Command(RegistrarTrabajo);

            CargarComisiones();
        }

        private void CargarComisiones()
        {
            try
            {
                var seleccionAnterior = ComisionSeleccionada;
                var comisiones = _comisionService.ListarComisiones();

                Comisiones.Clear();

                foreach (var comision in comisiones)
                {
                    Comisiones.Add(comision);
                }

                var seleccion = seleccionAnterior == null
                    ? null
                    : Comisiones.FirstOrDefault(c =>
                        c.Codigo == seleccionAnterior.Codigo);

                ComisionSeleccionada = seleccion ?? Comisiones.FirstOrDefault();

                if (ComisionSeleccionada == null)
                {
                    Eventos.Clear();
                    NotificarSinEventos();
                    Mensaje = "No hay comisiones registradas.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar las comisiones: {ex.Message}";
            }
        }

        private void CargarCalendario()
        {
            Eventos.Clear();
            NotificarSinEventos();

            if (ComisionSeleccionada == null)
            {
                return;
            }

            EstaCargando = true;

            try
            {
                var codigo = ComisionSeleccionada.Codigo;
                CargarMaterias(codigo);

                // Única fuente del calendario: el service une clases y
                // trabajos por fecha (antes lo duplicaba el ViewModel).
                foreach (var elemento in _calendarioService.VerCalendario(codigo))
                {
                    Eventos.Add(elemento);
                }

                CargarClasesYAsistencias(codigo);

                NotificarSinEventos();
                Mensaje = Eventos.Count == 0
                    ? "Todavía no hay eventos para esta comisión."
                    : string.Empty;
            }

            catch (Exception ex)
            {
                Mensaje = $"Error al cargar el calendario: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        private void CargarMaterias(string codigoComision)
        {
            var seleccionAnterior = MateriaSeleccionada?.Id;
            Materias.Clear();

            foreach (var materia in _materiaService.ListarMateriasDeComision(codigoComision))
                Materias.Add(materia);

            MateriaSeleccionada = (seleccionAnterior.HasValue
                ? Materias.FirstOrDefault(m => m.Id == seleccionAnterior.Value)
                : null) ?? Materias.FirstOrDefault();
        }

        private void CargarClasesYAsistencias(string codigoComision)
        {
            Clases.Clear();
            Asistencias.Clear();

            var clases = _claseService.ListarClases(codigoComision);
            var estudiantes = _estudianteService.ListarEstudiantes(codigoComision);
            foreach (var clase in clases)
            {
                var registros = _asistenciaService.ListarPorClase(clase.Id);
                var registrosPorEstudiante = registros
                    .GroupBy(a => a.EstudianteId)
                    .ToDictionary(g => g.Key, g => g.First().Condicion);

                var resumen = new ClaseCalendarioItem
                {
                    ClaseId = clase.Id,
                    Fecha = clase.Fecha,
                    Materias = string.IsNullOrWhiteSpace(clase.NombreMateria)
                        ? "Sin materia"
                        : clase.NombreMateria,
                    Estado = clase.Estado.ToString(),
                    Presentes = registros.Count(a => a.Condicion == Condicion.Presente),
                    Ausentes = registros.Count(a => a.Condicion == Condicion.Ausente),
                    Pendientes = Math.Max(0, estudiantes.Count - registros.Count)
                };

                Clases.Add(resumen);

                foreach (var estudiante in estudiantes)
                {
                    var condicion = registrosPorEstudiante.TryGetValue(
                        estudiante.Id,
                        out var registrada)
                        ? registrada
                        : Condicion.Pendiente;

                    Asistencias.Add(new AsistenciaCalendarioItem
                    {
                        ClaseId = clase.Id,
                        FechaClase = clase.Fecha,
                        Legajo = estudiante.Legajo,
                        Estudiante = $"{estudiante.Apellido}, {estudiante.Nombre}",
                        Condicion = condicion.ToString()
                    });
                }
            }
        }

        public sealed class ClaseCalendarioItem
        {
            public Guid ClaseId { get; set; }
            public DateTime Fecha { get; set; }
            public string Materias { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public int Presentes { get; set; }
            public int Ausentes { get; set; }
            public int Pendientes { get; set; }
        }

        public sealed class AsistenciaCalendarioItem
        {
            public Guid ClaseId { get; set; }
            public DateTime FechaClase { get; set; }
            public string Legajo { get; set; } = string.Empty;
            public string Estudiante { get; set; } = string.Empty;
            public string Condicion { get; set; } = string.Empty;
        }

        private void RegistrarClase()
        {
            if (ComisionSeleccionada == null)
            {
                Mensaje = "Seleccioná una comisión.";
                return;
            }

            if (MateriaSeleccionada == null)
            {
                Mensaje = "Seleccioná una materia.";
                return;
            }

            try
            {
                _claseService.AgregarClase(
                    ComisionSeleccionada.Codigo,
                    MateriaSeleccionada.Id,
                    FechaClase);

                CargarCalendario();

                Mensaje = "La clase se registró correctamente.";
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudo registrar la clase: {ex.Message}";
            }
        }

        private void RegistrarTrabajo()
        {
            if (ComisionSeleccionada == null)
            {
                Mensaje = "Seleccioná una comisión.";
                return;
            }

            if (MateriaSeleccionada == null)
            {
                Mensaje = "Seleccioná una materia.";
                return;
            }

            try
            {
                _trabajoService.AgregarTrabajo(
                    ComisionSeleccionada.Codigo,
                    MateriaSeleccionada.Id,
                    TituloTrabajo,
                    FechaEntrega);

                TituloTrabajo = string.Empty;

                CargarCalendario();

                Mensaje = "El trabajo se registró correctamente.";
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudo registrar el trabajo: {ex.Message}";
            }
        }

        private void NotificarSinEventos()
        {
            OnPropertyChanged(nameof(SinEventos));
        }

        private bool SetProperty<T>(
            ref T campo,
            T valor,
            [CallerMemberName] string? nombre = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
            {
                return false;
            }

            campo = valor;
            OnPropertyChanged(nombre);
            return true;
        }

        private void OnPropertyChanged(
            [CallerMemberName] string? nombre = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre));
        }
    }
}
