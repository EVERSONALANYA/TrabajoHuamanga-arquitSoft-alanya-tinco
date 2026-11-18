namespace TrabajoHuamanga.Dominio.Entidades;

public class Zona
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Distrito { get; private set; }
    public bool Activo { get; private set; } = true;

    private Zona() { }

    public static Zona Crear(string nombre, string? distrito = null)
    {
        return new Zona
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Distrito = distrito?.Trim(),
            Activo = true
        };
    }
}
