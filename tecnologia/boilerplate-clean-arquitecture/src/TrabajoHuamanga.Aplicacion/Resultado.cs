namespace TrabajoHuamanga.Aplicacion;

public record Resultado(bool Exito, string? Error = null)
{
    public static Resultado Ok() => new(true);
    public static Resultado Falla(string error) => new(false, error);
}
