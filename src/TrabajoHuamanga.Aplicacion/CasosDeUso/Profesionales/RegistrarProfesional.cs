using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;

public class RegistrarProfesional
{
    private readonly IProfesionalRepository _repo;
    private readonly IRubroRepository _rubroRepo;
    private readonly IZonaRepository _zonaRepo;

    public RegistrarProfesional(IProfesionalRepository repo, IRubroRepository rubroRepo, IZonaRepository zonaRepo)
    {
        _repo = repo;
        _rubroRepo = rubroRepo;
        _zonaRepo = zonaRepo;
    }

    public async Task<ProfesionalDto> EjecutarAsync(CrearProfesionalDto dto)
    {
        var rubro = await _rubroRepo.ObtenerPorIdAsync(dto.RubroId)
            ?? throw new ArgumentException("El rubro no existe.");
        var zona = await _zonaRepo.ObtenerPorIdAsync(dto.ZonaId)
            ?? throw new ArgumentException("La zona no existe.");

        var profesional = Profesional.Crear(
            dto.NombreCompleto, dto.Telefono, dto.Descripcion,
            dto.RubroId, dto.ZonaId, dto.FotoUrl);

        await _repo.AgregarAsync(profesional);

        return new ProfesionalDto(
            profesional.Id, profesional.NombreCompleto, profesional.Telefono,
            profesional.Descripcion, rubro.Nombre, zona.Nombre,
            profesional.FotoUrl, 0, 0, true);
    }
}
