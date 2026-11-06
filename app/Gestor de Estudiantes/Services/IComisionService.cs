using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Contrato de la fachada de comisiones (H1): alta de comisiones y
    /// listado.
    /// </summary>
    public interface IComisionService
    {
        /// <summary>Devuelve todas las comisiones ordenadas por código.</summary>
        List<Comision> ListarComisiones();

        /// <summary>
        /// Crea una comisión validando código y semanas (H1).
        /// </summary>
        /// <param name="codigo">Código visible (obligatorio, único).</param>
        /// <param name="semanas">Cantidad de semanas del cuatrimestre; por defecto <see cref="Comision.SemanasPorDefecto"/>.</param>
        /// <returns>La comisión creada.</returns>
        /// <exception cref="ArgumentException">Si el código está vacío o las semanas son menores a 1.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una comisión con ese código.</exception>
        Comision AgregarComision(string codigo, int semanas = Comision.SemanasPorDefecto);
    }
}
