namespace Gestor_de_Estudiantes.Entidades
{
    // Estado de una clase en el calendario: un día no hábil (feriado,
    // suspensión u otro problema) no recibe asistencia y no entra al
    // cálculo de riesgo.
    public enum EstadoClase
    {
        Habil,
        NoHabil,
    }
}
