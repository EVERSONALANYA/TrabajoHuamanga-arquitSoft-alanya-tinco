using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Infraestructura.Cache;

public class CacheBusquedaMemoria : ICacheBusqueda
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _claves = new();

    public CacheBusquedaMemoria(IMemoryCache cache) => _cache = cache;

    public Task<T?> ObtenerAsync<T>(string clave) where T : class
    {
        _cache.TryGetValue(clave, out T? valor);
        return Task.FromResult(valor);
    }

    public Task GuardarAsync<T>(string clave, T valor, TimeSpan? expiracion = null) where T : class
    {
        _cache.Set(clave, valor, expiracion ?? TimeSpan.FromMinutes(5));
        _claves[clave] = 0;
        return Task.CompletedTask;
    }

    public Task EliminarAsync(string clave)
    {
        if (clave.EndsWith("*"))
        {
            var prefijo = clave.TrimEnd('*');
            foreach (var k in _claves.Keys.Where(k => k.StartsWith(prefijo)).ToList())
            {
                _cache.Remove(k);
                _claves.TryRemove(k, out _);
            }
        }
        else
        {
            _cache.Remove(clave);
            _claves.TryRemove(clave, out _);
        }
        return Task.CompletedTask;
    }
}
