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
        private readonly ConstructorDeEventosDeCalendario _constructorDeEventos;

        private Comision? _comisionSeleccionada;
        private DateTime _fechaClase = DateTime.Today;
        private DateTime _fechaEntrega = DateTime.Today;
        private string _tituloTrabajo = string.Empty;
        private string _mensaje = string.Empty;
        private bool _estaCargando;

        public ObservableCollection<Comision> Comisiones { get; } = new();

        public ObservableCollection<EventoCalendario> Eventos { get; } = new();

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
            ConstructorDeEventosDeCalendario constructorDeEventos)
        {
            _comisionService = comisionService;
            _claseService = claseService;
            _trabajoService = trabajoService;
            _constructorDeEventos = constructorDeEventos;

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

                var eventos = _constructorDeEventos.Construir(
                    _claseService.ListarClases(codigo),
                    _trabajoService.ListarTrabajos(codigo));

                foreach (var evento in eventos)
                {
                    Eventos.Add(evento);
                }

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

        private void RegistrarClase()
        {
            if (ComisionSeleccionada == null)
            {
                Mensaje = "Seleccioná una comisión.";
                return;
            }

            try
            {
                _claseService.AgregarClase(
                    ComisionSeleccionada.Codigo,
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

            try
            {
                _trabajoService.AgregarTrabajo(
                    ComisionSeleccionada.Codigo,
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

    public class EventoCalendario
    {
        public DateTime Fecha { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string Detalle { get; set; } = string.Empty;
    }
}
