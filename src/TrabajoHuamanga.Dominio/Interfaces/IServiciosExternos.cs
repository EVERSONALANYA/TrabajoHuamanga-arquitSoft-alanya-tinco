namespace TrabajoHuamanga.Dominio.Interfaces;

/// <summary>
/// Puerto para el servicio de sugerencia de rubro mediante IA.
/// Implementado por el adaptador de OpenRouter en Infraestructura.
/// </summary>
public interface ISugeridorRubro
{
    Task<string> SugerirRubroAsync(string descripcionProblema);
}

/// <summary>
/// Puerto para generar enlace de contacto por WhatsApp.
/// Implementado por el adaptador de WhatsApp en Infraestructura.
/// </summary>
public interface IGeneradorEnlaceContacto
{
    string GenerarEnlace(string telefono, string nombreProfesional, string rubroNombre);
}

/// <summary>
/// Puerto para la caché de búsquedas.
/// Implementado por Redis en Infraestructura.
/// </summary>
public interface ICacheBusqueda
{
    Task<T?> ObtenerAsync<T>(string clave) where T : class;
    Task GuardarAsync<T>(string clave, T valor, TimeSpan? expiracion = null) where T : class;
    Task EliminarAsync(string clave);
}
