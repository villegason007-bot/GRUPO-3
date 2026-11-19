namespace Gestor_de_Estudiantes.Services;

/// <summary>
/// Resultado consolidado del reporte de riesgo de una comisión (H14).
/// </summary>
public class ReporteRiesgoComision
{
    public string CodigoComision { get; set; } = string.Empty;
    public int TotalEstudiantes { get; set; }
    public int CantidadEnRiesgo { get; set; }

    /// <summary>
    /// false cuando la comisión no tiene estudiantes o ninguno tiene asistencias/entregas cargadas (CA3).
    /// </summary>
    public bool TieneDatosSuficientes { get; set; }

    /// <summary>
    /// Mensaje listo para mostrar en la interfaz (ej: "7 de 30 estudiantes están en riesgo de abandonar" o aviso de falta de datos).
    /// </summary>
    public string MensajeEstado { get; set; } = string.Empty;
}
