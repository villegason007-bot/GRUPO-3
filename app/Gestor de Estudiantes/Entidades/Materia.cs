namespace Gestor_de_Estudiantes.Entidades
{
    /// <summary>
    /// Materia que se dicta en una o varias comisiones. La relación es de
    /// muchos a muchos con <see cref="Comision"/> a través de la tabla
    /// <c>materia_comision</c> (<see cref="MateriaComision"/>); el
    /// <see cref="Nombre"/> es único sin distinguir mayúsculas de
    /// minúsculas para no repetir la misma materia con distinta escritura.
    /// </summary>
    public class Materia
    {
        /// <summary>Identificador único de la materia.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Nombre de la materia (obligatorio, único sin distinguir mayúsculas).</summary>
        public string Nombre { get; set; } = string.Empty;
    }
}
