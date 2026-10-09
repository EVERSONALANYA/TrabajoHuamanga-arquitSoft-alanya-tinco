namespace TrabajoHuamanga.Dominio;

public class Profesional
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Nombre { get; }
    public string Rubro { get; }
    public string Distrito { get; }

    public Profesional(string nombre, string rubro, string distrito)
    {
        Nombre = nombre;
        Rubro = rubro;
        Distrito = distrito;
    }

    // Regla de negocio: el promedio ignora las calificaciones ocultas.
    public static decimal CalcularReputacion(IEnumerable<Calificacion> calificaciones)
    {
        var visibles = calificaciones.Where(c => !c.Oculta).ToList();
        if (visibles.Count == 0) return 0m;
        return (decimal)visibles.Average(c => c.Puntaje);
    }
}
