using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Repositories;

namespace Gestor_de_Estudiantes.Services
{
    public class RiesgoService
    {
        private readonly IRiesgoRepository _riesgos;
        private readonly IEstudianteRepository _estudiantes;

        public RiesgoService(IRiesgoRepository riesgos, IEstudianteRepository estudiantes)
        {
            _riesgos = riesgos;
            _estudiantes = estudiantes;
        }

        public double ObtenerUmbral()
            => _riesgos.ObtenerUmbral();

        public void GuardarUmbral(double umbral)
            => _riesgos.GuardarUmbral(umbral);

        public List<ResultadoRiesgo> CalcularRiesgos(string codigoComision)
        {
            var umbral = _riesgos.ObtenerUmbral();
            var resultados = new List<ResultadoRiesgo>();

            foreach (var estudiante in _estudiantes.ListarEstudiantes(codigoComision))
            {
                var (clasesHabiles, faltas, conRegistro) =
                    _riesgos.ContarAsistencias(estudiante.Id, codigoComision);
                var (vencidos, entregados) = _riesgos.ContarTrabajos(estudiante.Id, codigoComision);

                var resultado = CalculadoraDeRiesgo.Calcular(
                    conRegistro ? clasesHabiles : 0, faltas, vencidos, vencidos - entregados, umbral);
                resultado.EstudianteId = estudiante.Id;
                resultado.Legajo = estudiante.Legajo;
                resultado.Nombre = $"{estudiante.Nombre} {estudiante.Apellido}".Trim();
                resultados.Add(resultado);
            }

            return resultados;
        }
    }
}
