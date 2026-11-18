using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Catálogo global de materias: solo se listan, se crean y se eliminan.
    /// La asignación a comisiones y estudiantes se hace en sus propias vistas.
    /// </summary>
    public class MateriasViewModel : INotifyPropertyChanged
    {
        private readonly IMateriaService _materiaService;
        private string _mensaje = string.Empty;

        public ObservableCollection<Materia> Materias { get; } = new();

        public string Mensaje
        {
            get => _mensaje;
            private set
            {
                _mensaje = value;
                OnPropertyChanged();
            }
        }

        public MateriasViewModel(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        public void Cargar()
        {
            try
            {
                Mensaje = string.Empty;
                var resultado = _materiaService.ListarMaterias();
                Materias.Clear();
                foreach (var materia in resultado)
                    Materias.Add(materia);
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudieron cargar las materias: {ex.Message}";
            }
        }

        public bool Crear(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                Mensaje = "Ingresá el nombre de la materia.";
                return false;
            }

            try
            {
                _materiaService.CrearMateria(nombre.Trim());
                Cargar();
                Mensaje = "La materia se creó correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool Eliminar(Materia? materia)
        {
            if (materia is null)
            {
                Mensaje = "Seleccioná una materia.";
                return false;
            }

            try
            {
                if (!_materiaService.EliminarMateria(materia.Nombre))
                {
                    Mensaje = "No se encontró la materia que se desea eliminar.";
                    return false;
                }

                Cargar();
                Mensaje = "La materia se eliminó correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? nombre = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
