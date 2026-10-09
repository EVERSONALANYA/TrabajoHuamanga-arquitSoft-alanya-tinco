namespace TrabajoHuamanga.Dominio;

public class Calificacion
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid VecinoId { get; }
    public Guid ProfesionalId { get; }
    public int Puntaje { get; }
    public string Comentario { get; }
    public DateTime Fecha { get; }
    public bool Oculta { get; private set; }

    public Calificacion(Guid vecinoId, Guid profesionalId, int puntaje, string comentario)
    {
        VecinoId = vecinoId;
        ProfesionalId = profesionalId;
        Puntaje = puntaje;
        Comentario = comentario;
        Fecha = DateTime.UtcNow;
    }

    // La moderacion oculta la calificacion; ya no cuenta para la reputacion.
    public void Ocultar() => Oculta = true;
}
