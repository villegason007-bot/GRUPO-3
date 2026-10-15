
namespace Gestor_de_Estudiantes.Services
{
    public class ResultadoRiesgo
    {
        public Guid EstudianteId { get; set; }
        public string Legajo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public int ClasesHabiles { get; set; }
        public int Faltas { get; set; }
        public int TrabajosVencidos { get; set; }
        public int NoEntregados { get; set; }

        public int MaxFaltasPermitidas { get; set; }
        public int MaxTrabajosNoEntregadosPermitidos { get; set; }

        public int? Puntaje { get; set; }
        public double Umbral { get; set; }

        public bool SinInformacion => Puntaje is null;

        public bool SuperaLimiteFaltas =>
            ClasesHabiles > 0 &&
            Faltas > MaxFaltasPermitidas;

        public bool SuperaLimiteTrabajos =>
            TrabajosVencidos > 0 &&
            NoEntregados > MaxTrabajosNoEntregadosPermitidos;

        public bool EnRiesgo =>
            !SinInformacion &&
            (
                SuperaLimiteFaltas ||
                SuperaLimiteTrabajos ||
                Puntaje >= Umbral
            );

        public string Estado
        {
            get
            {
                if (SinInformacion)
                    return "Sin información";

                return EnRiesgo ? "En riesgo" : "Sin riesgo";
            }
        }

        public string MotivoRiesgo
        {
            get
            {
                if (SinInformacion)
                    return "No hay información suficiente para evaluar.";

                var motivos = new List<string>();

                if (SuperaLimiteFaltas)
                    motivos.Add("Superó el máximo de inasistencias.");

                if (SuperaLimiteTrabajos)
                    motivos.Add("Superó el máximo de trabajos no entregados.");

                if (Puntaje >= Umbral)
                    motivos.Add("Alcanzó el umbral porcentual de riesgo.");

                return motivos.Count > 0
                    ? string.Join(" ", motivos)
                    : "Está dentro de los límites permitidos.";
            }
        }
    }
}
