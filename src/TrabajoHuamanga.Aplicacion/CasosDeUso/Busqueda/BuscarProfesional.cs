using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;

public class BuscarProfesional
{
    private readonly IProfesionalRepository _repo;
    private readonly ICacheBusqueda _cache;

    public BuscarProfesional(IProfesionalRepository repo, ICacheBusqueda cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<List<ProfesionalDto>> EjecutarAsync(BusquedaDto dto)
    {
        // Generar clave de caché
        var clave = $"busqueda:{dto.RubroId}:{dto.ZonaId}:{dto.Termino?.ToLower()}";

        // Intentar obtener de caché (Redis)
        var cached = await _cache.ObtenerAsync<List<ProfesionalDto>>(clave);
        if (cached != null)
            return cached;

        // Si no está en caché, buscar en SQL Server
        var profesionales = await _repo.BuscarPorRubroYZonaAsync(dto.RubroId, dto.ZonaId, dto.Termino);

        var resultado = profesionales
            .Where(p => p.Activo)
            .Select(p => new ProfesionalDto(
                p.Id, p.NombreCompleto, p.Telefono, p.Descripcion,
                p.Rubro?.Nombre ?? "", p.Zona?.Nombre ?? "",
                p.FotoUrl, p.CalcularPromedioReputacion(), p.TotalCalificaciones(), p.Activo))
            .OrderByDescending(p => p.PromedioReputacion)
            .ThenByDescending(p => p.TotalCalificaciones)
            .ToList();

        // Guardar en caché por 5 minutos
        await _cache.GuardarAsync(clave, resultado, TimeSpan.FromMinutes(5));

        return resultado;
    }
}
