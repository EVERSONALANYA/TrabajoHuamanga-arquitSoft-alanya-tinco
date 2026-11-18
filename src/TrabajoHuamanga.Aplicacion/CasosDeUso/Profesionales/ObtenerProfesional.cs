using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;

public class ObtenerProfesional
{
    private readonly IProfesionalRepository _repo;

    public ObtenerProfesional(IProfesionalRepository repo) => _repo = repo;

    public async Task<ProfesionalDto?> EjecutarAsync(Guid id)
    {
        var p = await _repo.ObtenerPorIdAsync(id);
        if (p == null) return null;

        return new ProfesionalDto(
            p.Id, p.NombreCompleto, p.Telefono, p.Descripcion,
            p.Rubro?.Nombre ?? "", p.Zona?.Nombre ?? "",
            p.FotoUrl, p.CalcularPromedioReputacion(), p.TotalCalificaciones(), p.Activo);
    }

    public async Task<List<ProfesionalDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(p => new ProfesionalDto(
            p.Id, p.NombreCompleto, p.Telefono, p.Descripcion,
            p.Rubro?.Nombre ?? "", p.Zona?.Nombre ?? "",
            p.FotoUrl, p.CalcularPromedioReputacion(), p.TotalCalificaciones(), p.Activo
        )).ToList();
    }
}
