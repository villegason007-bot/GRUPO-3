using System.Collections.ObjectModel;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel
{
    /// <summary>
    /// Materias asignadas y disponibles de una comisión. Usa directamente
    /// <see cref="IMateriaService"/>, así que no depende de lo que tenga
    /// <c>ComisionesViewModel</c>.
    /// </summary>
    public class ComisionMateriasViewModel
    {
        private readonly IMateriaService _materiaService;

        public ObservableCollection<Materia> MateriasComision { get; } = new();

        public ObservableCollection<Materia> MateriasDisponibles { get; } = new();

        public string Mensaje { get; private set; } = string.Empty;

        public ComisionMateriasViewModel(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        /// <summary>Recarga las materias de la comisión y las que todavía se pueden asignar.</summary>
        public void Cargar(Comision? comision)
        {
            MateriasComision.Clear();
            MateriasDisponibles.Clear();

            if (comision is null)
                return;

            try
            {
                var asignadas = _materiaService.ListarMateriasDeComision(comision.Codigo);
                foreach (var materia in asignadas)
                    MateriasComision.Add(materia);

                var ids = asignadas.Select(m => m.Id).ToHashSet();
                foreach (var materia in _materiaService.ListarMaterias()
                             .Where(m => !ids.Contains(m.Id)))
                    MateriasDisponibles.Add(materia);
            }
            catch (Exception ex)
            {
                Mensaje = $"No se pudieron cargar las materias: {ex.Message}";
            }
        }

        public bool Asignar(Comision? comision, Materia? materia)
        {
            if (comision is null)
            {
                Mensaje = "Seleccioná una comisión.";
                return false;
            }

            if (materia is null)
            {
                Mensaje = "Seleccioná una materia.";
                return false;
            }

            try
            {
                _materiaService.AsignarMateria(materia.Nombre, comision.Codigo);
                Cargar(comision);
                Mensaje = "La materia se asignó correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool Desasignar(Comision? comision, Materia? materia)
        {
            if (comision is null || materia is null)
            {
                Mensaje = "Seleccioná una materia.";
                return false;
            }

            try
            {
                _materiaService.DesasignarMateria(materia.Nombre, comision.Codigo);
                Cargar(comision);
                Mensaje = "La materia se quitó de la comisión.";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }
    }
}