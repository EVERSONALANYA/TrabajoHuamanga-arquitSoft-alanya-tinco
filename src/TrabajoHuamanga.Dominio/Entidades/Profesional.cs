namespace TrabajoHuamanga.Dominio.Entidades;

public class Profesional
{
    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public Guid RubroId { get; private set; }
    public Rubro? Rubro { get; private set; }
    public Guid ZonaId { get; private set; }
    public Zona? Zona { get; private set; }
    public string? FotoUrl { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTime FechaRegistro { get; private set; }
    public List<Calificacion> Calificaciones { get; private set; } = new();

    private Profesional() { } // EF Core

    public static Profesional Crear(string nombre, string telefono, string descripcion, Guid rubroId, Guid zonaId, string? fotoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(telefono))
            throw new ArgumentException("El teléfono es obligatorio.");

        return new Profesional
        {
            Id = Guid.NewGuid(),
            NombreCompleto = nombre.Trim(),
            Telefono = telefono.Trim(),
            Descripcion = descripcion?.Trim() ?? string.Empty,
            RubroId = rubroId,
            ZonaId = zonaId,
            FotoUrl = fotoUrl,
            FechaRegistro = DateTime.UtcNow,
            Activo = true
        };
    }

    public double CalcularPromedioReputacion()
    {
        if (Calificaciones == null || Calificaciones.Count == 0)
            return 0;
        return Math.Round(Calificaciones.Where(c => c.Visible).Average(c => c.Puntaje), 1);
    }

    public int TotalCalificaciones()
    {
        return Calificaciones?.Count(c => c.Visible) ?? 0;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    public void ActualizarPerfil(string nombre, string telefono, string descripcion, Guid rubroId, Guid zonaId)
    {
        NombreCompleto = nombre.Trim();
        Telefono = telefono.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        RubroId = rubroId;
        ZonaId = zonaId;
    }
}
