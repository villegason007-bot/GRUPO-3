using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    public class ComisionesViewModel : INotifyPropertyChanged
    {
        private readonly IComisionService _comisionService;
        private readonly IEstudianteService _estudianteService;

        private Comision? _comisionSeleccionada;
        private bool _estaCargando;
        private string _mensaje = string.Empty;

        public ObservableCollection<Comision> Comisiones { get; } = new();

        public ObservableCollection<Estudiante> Estudiantes { get; } = new();

        public Comision? ComisionSeleccionada
        {
            get => _comisionSeleccionada;
            set
            {
                if (SetProperty(ref _comisionSeleccionada, value))
                {
                    CargarEstudiantes();
                    OnPropertyChanged(nameof(PuedeAgregarEstudiante));
                }
            }
        }

        public bool EstaCargando
        {
            get => _estaCargando;
            private set => SetProperty(ref _estaCargando, value);
        }

        public string Mensaje
        {
            get => _mensaje;
            private set => SetProperty(ref _mensaje, value);
        }

        public bool PuedeAgregarEstudiante =>
            ComisionSeleccionada is not null;

        public ICommand CargarComisionesCommand { get; }

        public ICommand AgregarComisionCommand { get; }

        public ICommand AgregarEstudianteCommand { get; }

        public ICommand RecargarEstudiantesCommand { get; }

        public ComisionesViewModel(
            IComisionService comisionService,
            IEstudianteService estudianteService)
        {
            _comisionService = comisionService;
            _estudianteService = estudianteService;

            CargarComisionesCommand =
                new Command(CargarComisiones);

            // Usar lambda que llama al método y descarta el bool devuelto
            AgregarComisionCommand =
                new Command<string>(codigo => { AgregarComision(codigo); });
        
            // Usar lambda que llama al método y descarta el bool devuelto
            AgregarEstudianteCommand =
                new Command<DatosNuevoEstudiante>(datos => { AgregarEstudiante(datos); });

            RecargarEstudiantesCommand =
                new Command(CargarEstudiantes);

            CargarComisiones();
        }

        public void CargarComisiones()
        {
            try
            {
                EstaCargando = true;
                Mensaje = string.Empty;

                var codigoSeleccionado =
                    ComisionSeleccionada?.Codigo;

                var resultado =
                    _comisionService.ListarComisiones();

                Comisiones.Clear();

                foreach (var comision in resultado)
                {
                    Comisiones.Add(comision);
                }

                ComisionSeleccionada =
                    Comisiones.FirstOrDefault(c =>
                        c.Codigo == codigoSeleccionado)
                    ?? Comisiones.FirstOrDefault();

                if (Comisiones.Count == 0)
                {
                    Estudiantes.Clear();
                    Mensaje = "Todavía no hay comisiones registradas.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudieron cargar las comisiones: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        public void CargarEstudiantes()
        {
            Estudiantes.Clear();

            if (ComisionSeleccionada is null)
            {
                OnPropertyChanged(nameof(Estudiantes));
                return;
            }

            try
            {
                EstaCargando = true;
                Mensaje = string.Empty;

                var resultado = _estudianteService.ListarEstudiantes(
                    ComisionSeleccionada.Codigo);

                foreach (var estudiante in resultado)
                {
                    Estudiantes.Add(estudiante);
                }

                if (Estudiantes.Count == 0)
                {
                    Mensaje = "Esta comisión todavía no tiene estudiantes.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudieron cargar los estudiantes: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        public bool AgregarComision(
            string? codigo,
            int semanas = Comision.SemanasPorDefecto)
        {
            try
            {
                Mensaje = string.Empty;

                var nueva = _comisionService.AgregarComision(
                    codigo ?? string.Empty,
                    semanas);

                Comisiones.Add(nueva);
                ComisionSeleccionada = nueva;

                Mensaje = "La comisión se creó correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool AgregarEstudiante(
            DatosNuevoEstudiante? datos)
        {
            if (datos is null)
            {
                Mensaje = "No se recibieron los datos del estudiante.";
                return false;
            }

            if (ComisionSeleccionada is null)
            {
                Mensaje = "Seleccioná una comisión.";
                return false;
            }

            try
            {
                Mensaje = string.Empty;

                _estudianteService.AgregarEstudiante(
                    datos.Legajo,
                    datos.Nombre,
                    ComisionSeleccionada.Codigo);

                CargarEstudiantes();

                Mensaje = "El estudiante se agregó correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Crea una comisión desde el texto del formulario (código y
        /// duración en semanas como texto), validando la duración antes de
        /// delegar en <see cref="AgregarComision(string?, int)"/>.
        /// </summary>
        public bool AgregarComision(string? codigo, string? semanasTexto)
        {
            if (!int.TryParse(semanasTexto, out var semanas) || semanas < 1)
            {
                Mensaje = "La duración debe ser de al menos una semana.";
                return false;
            }

            return AgregarComision(codigo, semanas);
        }

        /// <summary>
        /// Agrega un estudiante a la comisión indicada desde el formulario,
        /// validando los campos obligatorios y seleccionando la comisión
        /// antes de delegar en <see cref="AgregarEstudiante(DatosNuevoEstudiante?)"/>.
        /// </summary>
        public bool AgregarEstudiante(
            string? legajo,
            string? nombre,
            Comision? comision)
        {
            if (string.IsNullOrWhiteSpace(legajo))
            {
                Mensaje = "Ingresá el legajo del estudiante.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Mensaje = "Ingresá el nombre del estudiante.";
                return false;
            }

            if (comision is null)
            {
                Mensaje = "Seleccioná una comisión.";
                return false;
            }

            // Usar la comisión que se está consultando.
            ComisionSeleccionada =
                Comisiones.FirstOrDefault(c => c.Id == comision.Id)
                ?? comision;

            return AgregarEstudiante(new DatosNuevoEstudiante
            {
                Legajo = legajo.Trim(),
                Nombre = nombre.Trim()
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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

    public class DatosNuevoEstudiante
    {
        public string Legajo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
    }
}