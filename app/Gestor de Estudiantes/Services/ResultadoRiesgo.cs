namespace Gestor_de_Estudiantes.Services
{
    // Resultado del cálculo de riesgo de un estudiante (H6).
    // Puntaje null = sin información suficiente (Escenario 2 de H6).
    public class ResultadoRiesgo
    {
        public Guid EstudianteId { get; set; }
        public string Legajo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public int ClasesHabiles { get; set; }
        public int Faltas { get; set; }
        public int TrabajosVencidos { get; set; }
        public int NoEntregados { get; set; }

        public int? Puntaje { get; set; }
        public double Umbral { get; set; }

        public bool EnRiesgo => Puntaje is not null && Puntaje >= Umbral;
        public bool SinInformacion => Puntaje is null;
    }
}
