using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Infraestructura.Cache;

/// <summary>
/// Implementación de caché usando Redis.
/// Búsquedas frecuentes se guardan 5 minutos → respuesta ~2ms vs ~200ms.
/// </summary>
public class CacheBusquedaRedis : ICacheBusqueda
{
    private readonly IDistributedCache _cache;

    public CacheBusquedaRedis(IDistributedCache cache) => _cache = cache;

    public async Task<T?> ObtenerAsync<T>(string clave) where T : class
    {
        try
        {
            var data = await _cache.GetStringAsync(clave);
            if (data == null) return null;
            return JsonSerializer.Deserialize<T>(data);
        }
        catch
        {
            // Si Redis no está disponible, retornar null (degradación elegante)
            return null;
        }
    }

    public async Task GuardarAsync<T>(string clave, T valor, TimeSpan? expiracion = null) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(valor);
            var opciones = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiracion ?? TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(clave, json, opciones);
        }
        catch
        {
            // Si Redis falla, la app sigue funcionando sin caché
        }
    }

    public async Task EliminarAsync(string clave)
    {
        try
        {
            await _cache.RemoveAsync(clave);
        }
        catch
        {
            // Degradación elegante
        }
    }
}
