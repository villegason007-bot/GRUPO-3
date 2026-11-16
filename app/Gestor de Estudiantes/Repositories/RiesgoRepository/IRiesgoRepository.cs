using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de los conteos que alimentan el cálculo de riesgo (H6):
    /// asistencias en clases hábiles y trabajos vencidos. Los parámetros
    /// configurables (H7) viven en <see cref="IParametrosRiesgoRepository"/>.
    /// </summary>
    public interface IRiesgoRepository
    {
        /// <summary>
        /// Cuenta asistencia de un estudiante en una comisión, considerando
        /// solo clases hábiles.
        /// </summary>
        /// <param name="estudianteId">Estudiante a evaluar.</param>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>
        /// (clases hábiles de la comisión, faltas del estudiante,
        /// si tiene alguna asistencia registrada en clases hábiles).
        /// </returns>
        (int ClasesHabiles, int Faltas, bool TieneRegistro) ContarAsistencias(
            Guid estudianteId, string codigoComision);

        /// <summary>Cuenta trabajos ya vencidos a hoy en la comisión y cuántos entregó el estudiante.</summary>
        /// <param name="estudianteId">Estudiante a evaluar.</param>
        /// <param name="codigoComision">Comisión a evaluar.</param>
        /// <returns>(trabajos vencidos, de esos cuántos entregó el estudiante).</returns>
        (int TrabajosVencidos, int Entregados) ContarTrabajos(Guid estudianteId, string codigoComision);
    }
}
