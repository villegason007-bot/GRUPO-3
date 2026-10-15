
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class AsistenciaService
    {
        private readonly IAsistenciaRepository _asistencias;

        public AsistenciaService(IAsistenciaRepository asistencias)
        {
            _asistencias = asistencias;
        }

        public List<Asistencia> ListarPorClase(Guid claseId)
        {
            return _asistencias.ListarPorClase(claseId);
        }

        public void GuardarAsistencia(
            Guid claseId,
            Guid estudianteId,
            bool presente)
        {
            var condicion = presente
                ? Condicion.Presente
                : Condicion.Ausente;

            var existentes = _asistencias.ListarPorClase(claseId);

            var asistencia = existentes.FirstOrDefault(a =>
                a.EstudianteId == estudianteId);

            if (asistencia != null)
            {
                _asistencias.ActualizarCondicion(
                    claseId,
                    estudianteId,
                    condicion);

                return;
            }

            _asistencias.RegistrarAsistencia(new Asistencia
            {
                ClaseId = claseId,
                EstudianteId = estudianteId,
                Condicion = condicion
            });
        }
    }
}
