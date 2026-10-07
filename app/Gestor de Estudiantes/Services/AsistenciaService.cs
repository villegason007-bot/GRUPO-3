using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Services;

internal class AsistenciaService
{
    // Método para generar la lista de asistencia en blanco para una clase
    public List<Asistencia> GenerarPlanillaAsistencia(Clase clase, List<Estudiante> estudiantes)
    {
        var planilla = new List<Asistencia>();

        foreach (var estudiante in estudiantes)
        {
            planilla.Add(new Asistencia
            {
                Clase = clase,
                Estudiante = estudiante,
                Condicion = Condicion.Ausente // Por defecto arrancan ausentes hasta que se marque el casillero
            });
        }

        return planilla;
    }

    // Método para guardar la asistencia (Para el Sprint 1 alcanza con simular el guardado)
    public bool GuardarAsistencia(List<Asistencia> planilla)
    {
        // Aquí en sprints futuros se conectará a una base de datos o API
        return planilla != null && planilla.Count > 0;
    }
}
