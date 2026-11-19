namespace Gestor_de_Estudiantes.Services;

/// <summary>
/// Implementación del reporte de riesgo por comisión (H14).
/// Reutiliza <see cref="IRiesgoService"/> para garantizar coherencia total con el panel (CA2).
/// </summary>
public class ReporteRiesgoService : IReporteRiesgoService
{
    private readonly IRiesgoService _riesgoService;

    public ReporteRiesgoService(IRiesgoService riesgoService)
    {
        _riesgoService = riesgoService ?? throw new ArgumentNullException(nameof(riesgoService));
    }

    public ReporteRiesgoComision ObtenerReportePorComision(string codigoComision)
    {
        if (string.IsNullOrWhiteSpace(codigoComision))
            throw new ArgumentException("El código de la comisión es obligatorio.", nameof(codigoComision));

        // CA2: Usamos exactamente el mismo cálculo que alimenta al panel principal
        List<ResultadoRiesgo> resultados = _riesgoService.CalcularRiesgos(codigoComision);

        int totalEstudiantes = resultados.Count;

        // Caso borde: comisión sin alumnos inscriptos
        if (totalEstudiantes == 0)
        {
            return new ReporteRiesgoComision
            {
                CodigoComision = codigoComision,
                TotalEstudiantes = 0,
                CantidadEnRiesgo = 0,
                TieneDatosSuficientes = false,
                MensajeEstado = $"La comisión {codigoComision} no tiene estudiantes cargados."
            };
        }

        // CA3: Si TODOS los estudiantes tienen SinInformacion == true, no hay asistencias ni entregas registradas aún
        bool hayAlgunDatoCargado = resultados.Any(r => !r.SinInformacion);

        if (!hayAlgunDatoCargado)
        {
            return new ReporteRiesgoComision
            {
                CodigoComision = codigoComision,
                TotalEstudiantes = totalEstudiantes,
                CantidadEnRiesgo = 0,
                TieneDatosSuficientes = false,
                MensajeEstado = $"Todavía no hay datos (asistencias ni entregas) para calcular el riesgo en {codigoComision}."
            };
        }

        // CA1: Contamos cuántos estudiantes tienen la propiedad EnRiesgo en true
        int enRiesgo = resultados.Count(r => r.EnRiesgo);

        return new ReporteRiesgoComision
        {
            CodigoComision = codigoComision,
            TotalEstudiantes = totalEstudiantes,
            CantidadEnRiesgo = enRiesgo,
            TieneDatosSuficientes = true,
            MensajeEstado = $"{enRiesgo} de {totalEstudiantes} estudiantes están en riesgo de abandonar."
        };
    }
}
