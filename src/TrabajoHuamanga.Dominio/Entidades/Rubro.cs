namespace TrabajoHuamanga.Dominio.Entidades;

public class Rubro
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string? Icono { get; private set; }
    public bool Activo { get; private set; } = true;

    private Rubro() { }

    public static Rubro Crear(string nombre, string? descripcion = null, string? icono = null)
    {
        return new Rubro
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Icono = icono,
            Activo = true
        };
    }
}
