namespace Gestor_de_Estudiantes.Services
{
    /// <summary>
    /// Resultado del cálculo de riesgo de un estudiante (H6) con todo lo
    /// que la UI necesita: conteos, límites configurables, puntaje,
    /// estado y motivo.
    ///
    /// El estado es híbrido: está en riesgo si supera el máximo de
    /// faltas, si supera el máximo de trabajos no entregados, o si su
    /// puntaje alcanza el umbral porcentual.
    /// </summary>
    public class ResultadoRiesgo
    {
        /// <summary>Estudiante al que pertenece este resultado.</summary>
        public Guid EstudianteId { get; set; }

        /// <summary>Legajo del estudiante (para mostrar en el panel).</summary>
        public string Legajo { get; set; } = string.Empty;

        /// <summary>Nombre completo del estudiante (para mostrar en el panel).</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Clases hábiles de la comisión (denominador de faltas).</summary>
        public int ClasesHabiles { get; set; }

        /// <summary>Ausencias del estudiante en clases hábiles.</summary>
        public int Faltas { get; set; }

        /// <summary>Trabajos de la comisión ya vencidos a hoy (denominador de entregas).</summary>
        public int TrabajosVencidos { get; set; }

        /// <summary>De los vencidos, cuántos no entregó el estudiante.</summary>
        public int NoEntregados { get; set; }

        /// <summary>Máximo de inasistencias permitidas (configurable desde la UI; límite absoluto).</summary>
        public int MaxFaltasPermitidas { get; set; }

        /// <summary>Máximo de trabajos no entregados permitidos (configurable desde la UI; límite absoluto).</summary>
        public int MaxTrabajosNoEntregadosPermitidos { get; set; }

        /// <summary>Puntaje 0-100; null cuando no hay datos suficientes para calcular.</summary>
        public int? Puntaje { get; set; }

        /// <summary>Umbral porcentual (0-100) a partir del cual el puntaje se considera en riesgo.</summary>
        public double Umbral { get; set; }

        /// <summary>true si no había ningún componente con datos (Escenario 2 de H6).</summary>
        public bool SinInformacion => Puntaje is null;

        /// <summary>true si las faltas superan el máximo permitido (y hay clases hábiles cargadas).</summary>
        public bool SuperaLimiteFaltas =>
            ClasesHabiles > 0 &&
            Faltas > MaxFaltasPermitidas;

        /// <summary>true si los no entregados superan el máximo permitido (y hay trabajos vencidos).</summary>
        public bool SuperaLimiteTrabajos =>
            TrabajosVencidos > 0 &&
            NoEntregados > MaxTrabajosNoEntregadosPermitidos;

        /// <summary>
        /// Decisión de riesgo: true si hay información y se supera alguno
        /// de los tres criterios (límite de faltas, límite de trabajos o
        /// umbral porcentual).
        /// </summary>
        public bool EnRiesgo =>
            !SinInformacion &&
            (
                SuperaLimiteFaltas ||
                SuperaLimiteTrabajos ||
                Puntaje >= Umbral
            );

        /// <summary>Texto de estado para la UI: «Sin información», «En riesgo» o «Sin riesgo».</summary>
        public string Estado
        {
            get
            {
                if (SinInformacion)
                    return "Sin información";

                return EnRiesgo ? "En riesgo" : "Sin riesgo";
            }
        }

        /// <summary>
        /// Explicación en texto de por qué está (o no) en riesgo: nombra
        /// cada criterio superado; si no hay datos, lo aclaro.
        /// </summary>
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
