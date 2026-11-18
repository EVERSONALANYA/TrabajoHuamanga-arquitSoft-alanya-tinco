namespace TrabajoHuamanga.Dominio.Entidades;

public class Calificacion
{
    public Guid Id { get; private set; }
    public Guid ProfesionalId { get; private set; }
    public Profesional? Profesional { get; private set; }
    public string NombreUsuario { get; private set; } = string.Empty;
    public int Puntaje { get; private set; }
    public string? Comentario { get; private set; }
    public bool Visible { get; private set; } = true;
    public bool Reportada { get; private set; } = false;
    public string? MotivoReporte { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Calificacion() { } // EF Core

    public static Calificacion Crear(Guid profesionalId, string nombreUsuario, int puntaje, string? comentario)
    {
        if (puntaje < 1 || puntaje > 5)
            throw new ArgumentException("El puntaje debe estar entre 1 y 5.");
        if (string.IsNullOrWhiteSpace(nombreUsuario))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        return new Calificacion
        {
            Id = Guid.NewGuid(),
            ProfesionalId = profesionalId,
            NombreUsuario = nombreUsuario.Trim(),
            Puntaje = puntaje,
            Comentario = comentario?.Trim(),
            Visible = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public void Reportar(string motivo)
    {
        Reportada = true;
        MotivoReporte = motivo;
    }

    public void OcultarPorModeracion() => Visible = false;
    public void Restaurar()
    {
        Visible = true;
        Reportada = false;
        MotivoReporte = null;
    }
}
