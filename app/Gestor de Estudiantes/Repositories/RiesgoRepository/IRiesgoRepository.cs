using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    /// <summary>
    /// Contrato de acceso a los parámetros del indicador de riesgo y a
    /// los conteos que alimentan su cálculo (H6/H7/H8).
    /// </summary>
    public interface IRiesgoRepository
    {
        /// <summary>Devuelve el umbral porcentual vigente (0-100).</summary>
        double ObtenerUmbral();

        /// <summary>Guarda el umbral porcentual (0-100).</summary>
        /// <param name="umbral">Valor a guardar.</param>
        /// <exception cref="ArgumentException">Si está fuera de 0-100.</exception>
        void GuardarUmbral(double umbral);

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

        /// <summary>Devuelve el máximo de inasistencias permitidas (por defecto 3 si no está guardado).</summary>
        int ObtenerMaxFaltasPermitidas();

        /// <summary>Guarda el máximo de inasistencias permitidas.</summary>
        /// <param name="cantidad">Nuevo máximo (no puede ser negativo).</param>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxFaltasPermitidas(int cantidad);

        /// <summary>Devuelve el máximo de trabajos no entregados permitidos (por defecto 2 si no está guardado).</summary>
        int ObtenerMaxTrabajosNoEntregadosPermitidos();

        /// <summary>Guarda el máximo de trabajos no entregados permitidos.</summary>
        /// <param name="cantidad">Nuevo máximo (no puede ser negativo).</param>
        /// <exception cref="ArgumentException">Si es negativo.</exception>
        void GuardarMaxTrabajosNoEntregadosPermitidos(int cantidad);
    }
}
