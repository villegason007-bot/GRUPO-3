using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Fachada de materias (QA001): alta de materias con nombre único sin
    /// distinguir mayúsculas y gestión del vínculo N:M con comisiones.
    /// </summary>
    public class MateriaService : IMateriaService
    {
        private readonly IMateriaRepository _materias;
        private readonly IComisionRepository _comisiones;

        /// <summary>Crea el servicio con los repositorios de materias y comisiones.</summary>
        public MateriaService(
            IMateriaRepository materias,
            IComisionRepository comisiones)
        {
            _materias = materias;
            _comisiones = comisiones;
        }

        /// <summary>Devuelve todas las materias ordenadas por nombre.</summary>
        public List<Materia> ListarMaterias()
            => _materias.ListarMaterias();

        /// <summary>
        /// Crea una materia validando nombre obligatorio y único sin
        /// distinguir mayúsculas (MATEMATICAS ≡ Matematicas).
        /// </summary>
        /// <exception cref="ArgumentException">Si el nombre está vacío.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una materia con ese nombre.</exception>
        public Materia CrearMateria(string nombre)
        {
            nombre = (nombre ?? string.Empty).Trim();

            if (nombre.Length == 0)
                throw new ArgumentException("El nombre de la materia es obligatorio.");
            if (_materias.ExisteMateria(nombre))
                throw new InvalidOperationException($"Ya existe la materia {nombre}.");

            var materia = new Materia { Nombre = nombre };
            _materias.AgregarMateria(materia);
            return materia;
        }

        /// <summary>Asigna una materia a una comisión (N:M).</summary>
        /// <exception cref="InvalidOperationException">Si no existen la materia o la comisión.</exception>
        public void AsignarMateria(string nombreMateria, string codigoComision)
        {
            nombreMateria = (nombreMateria ?? string.Empty).Trim();
            codigoComision = (codigoComision ?? string.Empty).Trim();

            if (nombreMateria.Length == 0)
                throw new ArgumentException("El nombre de la materia es obligatorio.");
            if (!_materias.ExisteMateria(nombreMateria))
                throw new InvalidOperationException($"No existe la materia {nombreMateria}.");
            if (!_comisiones.ExisteComision(codigoComision))
                throw new InvalidOperationException($"No existe la comisión {codigoComision}.");

            _materias.AsignarMateria(nombreMateria, codigoComision);
        }

        /// <summary>Desasigna una materia de una comisión.</summary>
        public bool DesasignarMateria(string nombreMateria, string codigoComision)
        {
            nombreMateria = (nombreMateria ?? string.Empty).Trim();
            codigoComision = (codigoComision ?? string.Empty).Trim();

            if (nombreMateria.Length == 0 || codigoComision.Length == 0)
                throw new ArgumentException("Materia y comisión son obligatorias.");

            return _materias.DesasignarMateria(nombreMateria, codigoComision);
        }

        /// <summary>Devuelve las materias de la comisión ordenadas por nombre.</summary>
        public List<Materia> ListarMateriasDeComision(string codigoComision)
            => _materias.ListarMateriasDeComision((codigoComision ?? string.Empty).Trim());

        /// <summary>Devuelve las comisiones donde se dicta la materia.</summary>
        public List<Comision> ListarComisionesDeMateria(string nombreMateria)
            => _materias.ListarComisionesDeMateria((nombreMateria ?? string.Empty).Trim());
    }
}
