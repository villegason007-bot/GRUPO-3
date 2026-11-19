namespace Gestor_de_Estudiantes.Services;

/// <summary>
/// Contrato del servicio de reporte de riesgo por comisión (H14).
/// </summary>
public interface IReporteRiesgoService
{
    /// <summary>
    /// Genera el resumen de estudiantes en riesgo para una comisión dada.
    /// </summary>
    /// <param name="codigoComision">Código visible de la comisión (ej: "1°A").</param>
    ReporteRiesgoComision ObtenerReportePorComision(string codigoComision);
}
