using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Ficha del estudiante (H9) y registro de contactos (H10): compone
    /// los datos identificatorios y de contacto con el historial de
    /// contactos del tutor, y valida el formulario del modal antes de
    /// delegar en <see cref="IContactoService"/>. Además permite inscribir
    /// al estudiante en materias de su comisión y quitarlas.
    /// </summary>
    public class FichaEstudianteViewModel : INotifyPropertyChanged
    {
        private readonly IFichaService _fichas;
        private readonly IContactoService _contactos;
        private readonly IMateriaService _materias;
        private readonly IInscripcionMateriaService _inscripciones;

        private string _nombreCompleto = string.Empty;
        private string _subtitulo = string.Empty;
        private string _telefonoTexto = string.Empty;
        private string _fechaIncorporacionTexto = string.Empty;
        private string _mensaje = string.Empty;
        private bool _mostrarMensaje;
        private string _legajo = string.Empty;
        private Guid _estudianteId;
        private string _codigoComision = string.Empty;

        public FichaEstudianteViewModel(
            IFichaService fichas,
            IContactoService contactos,
            IMateriaService materias,
            IInscripcionMateriaService inscripciones)
        {
            _fichas = fichas;
            _contactos = contactos;
            _materias = materias;
            _inscripciones = inscripciones;
        }

        /// <summary>Historial de contactos del estudiante (H10), del más reciente al más antiguo.</summary>
        public ObservableCollection<ContactoFilaViewModel> Contactos { get; } = new();

        /// <summary>Materias en las que el estudiante está inscripto.</summary>
        public ObservableCollection<Materia> MateriasInscriptas { get; } = new();

        /// <summary>Materias de su comisión en las que todavía no está inscripto.</summary>
        public ObservableCollection<Materia> MateriasAsignables { get; } = new();

        /// <summary>Opciones de medio del modal de contacto (H10).</summary>
        public List<string> Medios { get; } = new() { "Llamada", "Mensaje" };

        /// <summary>Opciones de resultado del modal de contacto (H10).</summary>
        public List<string> Resultados { get; } = new()
        {
            "Contestó",
            "Quedó en veremos",
            "Sin respuesta",
        };

        /// <summary>Nombre completo del estudiante de la ficha.</summary>
        public string NombreCompleto
        {
            get => _nombreCompleto;
            private set => Notificar(ref _nombreCompleto, value);
        }

        /// <summary>Legajo y comisión, para el subtítulo de la ficha.</summary>
        public string Subtitulo
        {
            get => _subtitulo;
            private set => Notificar(ref _subtitulo, value);
        }

        /// <summary>Teléfono a mostrar («Sin teléfono» si no cargó).</summary>
        public string TelefonoTexto
        {
            get => _telefonoTexto;
            private set => Notificar(ref _telefonoTexto, value);
        }

        /// <summary>Fecha de incorporación a mostrar («—» si no cargó).</summary>
        public string FechaIncorporacionTexto
        {
            get => _fechaIncorporacionTexto;
            private set => Notificar(ref _fechaIncorporacionTexto, value);
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
        /// Carga la ficha del estudiante (H9), su historial de contactos
        /// (H10) y sus materias. Ante un legajo inexistente deja el
        /// mensaje de error.
        /// </summary>
        /// <param name="legajo">Legajo del estudiante a mostrar.</param>
        public void CargarFicha(string legajo)
        {
            try
            {
                var ficha = _fichas.ObtenerFicha(legajo);
                _legajo = ficha.Legajo;
                _estudianteId = ficha.Id;
                _codigoComision = ficha.CodigoComision;

                NombreCompleto = ficha.NombreCompleto;
                Subtitulo = $"Legajo {ficha.Legajo} · Comisión {ficha.CodigoComision}";
                TelefonoTexto = string.IsNullOrWhiteSpace(ficha.Telefono)
                    ? "Sin teléfono"
                    : ficha.Telefono;
                FechaIncorporacionTexto = FormatearFecha(ficha.FechaIncorporacion);

                Contactos.Clear();
                foreach (var contacto in ficha.Contactos)
                    Contactos.Add(new ContactoFilaViewModel(contacto));

                CargarMaterias();

                Mensaje = string.Empty;
                MostrarMensaje = false;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Mensaje = ex.Message;
                MostrarMensaje = true;
            }
        }

        // ---------------- Materias del estudiante ----------------

        /// <summary>
        /// Recarga las materias inscriptas y las que todavía se pueden
        /// asignar (solo las de la comisión del estudiante).
        /// </summary>
        public void CargarMaterias()
        {
            MateriasInscriptas.Clear();
            MateriasAsignables.Clear();

            if (_estudianteId == Guid.Empty)
                return;

            var inscriptas = _inscripciones.ListarMateriasDeEstudiante(_estudianteId);
            foreach (var materia in inscriptas)
                MateriasInscriptas.Add(materia);

            var ids = inscriptas.Select(m => m.Id).ToHashSet();
            foreach (var materia in _materias.ListarMateriasDeComision(_codigoComision)
                         .Where(m => !ids.Contains(m.Id)))
                MateriasAsignables.Add(materia);
        }

        /// <summary>Inscribe al estudiante en la materia elegida.</summary>
        public bool InscribirEnMateria(Materia? materia, out string mensaje)
        {
            if (materia is null)
            {
                mensaje = "Seleccioná una materia.";
                return false;
            }

            try
            {
                _inscripciones.InscribirEstudiante(_estudianteId, materia.Id);
                CargarMaterias();
                mensaje = $"Se inscribió en {materia.Nombre}.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        /// <summary>Quita la inscripción del estudiante en la materia.</summary>
        public bool DesinscribirDeMateria(Materia? materia, out string mensaje)
        {
            if (materia is null)
            {
                mensaje = "Seleccioná una materia.";
                return false;
            }

            try
            {
                _inscripciones.DesinscribirEstudiante(_estudianteId, materia.Id);
                CargarMaterias();
                mensaje = $"Se quitó la inscripción en {materia.Nombre}.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        // ---------------- Contactos ----------------

        /// <summary>
        /// Valida el formulario del modal y registra el contacto con la
        /// fecha del día (H10); al éxito recarga la ficha con el nuevo
        /// historial.
        /// </summary>
        /// <param name="medio">Medio elegido en el modal (o null si no se eligió).</param>
        /// <param name="resultado">Resultado elegido en el modal (o null si no se eligió).</param>
        /// <returns>null si se registró; el mensaje de error si la validación falló.</returns>
        public string? RegistrarContacto(string? medio, string? resultado)
        {
            if (_legajo.Length == 0)
                return "No hay ningún estudiante cargado.";

            if (medio is null)
                return "Seleccioná el medio del contacto.";

            if (resultado is null)
                return "Seleccioná el resultado del contacto.";

            try
            {
                _contactos.RegistrarContacto(
                    _legajo,
                    AMedioContacto(medio),
                    AResultadoContacto(resultado));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return ex.Message;
            }

            CargarFicha(_legajo);
            Mensaje = "Contacto registrado.";
            MostrarMensaje = true;
            return null;
        }

        /// <summary>Traduce la opción elegida del medio a su valor de dominio.</summary>
        private static MedioContacto AMedioContacto(string medio)
            => medio == "Mensaje"
                ? MedioContacto.Mensaje
                : MedioContacto.Llamada;

        /// <summary>Traduce la opción elegida del resultado a su valor de dominio.</summary>
        private static ResultadoContacto AResultadoContacto(string resultado)
            => resultado switch
            {
                "Quedó en veremos" => ResultadoContacto.QuedoEnVeremos,
                "Sin respuesta" => ResultadoContacto.SinRespuesta,
                _ => ResultadoContacto.Contesto,
            };

        /// <summary>Formatea la fecha ISO de incorporación como dd/MM/aaaa (o «—» si no hay).</summary>
        private static string FormatearFecha(string? fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
                return "—";

            return DateTime.TryParse(fecha, out var fechaParseada)
                ? fechaParseada.ToString("dd/MM/yyyy")
                : fecha;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notificar<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return;

            campo = valor;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
    }

    /// <summary>Fila de historial de contactos (H10) con textos ya formateados para la vista.</summary>
    public class ContactoFilaViewModel
    {
        public ContactoFilaViewModel(Contacto contacto)
        {
            FechaTexto = contacto.Fecha.ToString("dd/MM/yyyy");

            Medio = contacto.Medio == MedioContacto.Llamada
                ? "Llamada"
                : "Mensaje";

            Resultado = contacto.Resultado switch
            {
                ResultadoContacto.Contesto => "Contestó",
                ResultadoContacto.QuedoEnVeremos => "Quedó en veremos",
                _ => "Sin respuesta",
            };
        }

        /// <summary>Fecha del contacto (dd/MM/aaaa).</summary>
        public string FechaTexto { get; }

        /// <summary>Medio utilizado.</summary>
        public string Medio { get; }

        /// <summary>Resultado obtenido.</summary>
        public string Resultado { get; }
    }
}