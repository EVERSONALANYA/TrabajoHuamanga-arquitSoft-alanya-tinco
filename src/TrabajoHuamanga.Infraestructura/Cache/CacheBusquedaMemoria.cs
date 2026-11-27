using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Infraestructura.Cache;

public class CacheBusquedaMemoria : ICacheBusqueda
{
    private readonly IMemoryCache _cache;
    public CacheBusquedaMemoria(IMemoryCache cache) => _cache = cache;

    public Task<T?> ObtenerAsync<T>(string clave) where T : class
    {
        try
        {
            if (_cache.TryGetValue(clave, out string? data) && data != null)
                return Task.FromResult(JsonSerializer.Deserialize<T>(data));
            return Task.FromResult<T?>(null);
        }
        catch { return Task.FromResult<T?>(null); }
    }

    public Task GuardarAsync<T>(string clave, T valor, TimeSpan? expiracion = null) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(valor);
            var opciones = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiracion ?? TimeSpan.FromMinutes(5)
            };
            _cache.Set(clave, json, opciones);
        }
        catch { }
        return Task.CompletedTask;
    }

    public Task EliminarAsync(string clave)
    {
        try { _cache.Remove(clave); } catch { }
        return Task.CompletedTask;
    }
}
