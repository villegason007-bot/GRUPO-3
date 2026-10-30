namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Vínculo entre <see cref="Materia"/> y <see cref="Comision"/>: una
    /// comisión puede tener muchas materias y una materia puede estar en
    /// muchas comisiones (muchos a muchos). Cada fila es una asignación
    /// única, identificada por la pareja de identificadores.
    /// </summary>
    public class MateriaComision
    {
        /// <summary>Identificador de la materia asignada.</summary>
        public Guid MateriaId { get; set; }

        /// <summary>Identificador de la comisión a la que se asigna.</summary>
        public Guid ComisionId { get; set; }
    }
}
