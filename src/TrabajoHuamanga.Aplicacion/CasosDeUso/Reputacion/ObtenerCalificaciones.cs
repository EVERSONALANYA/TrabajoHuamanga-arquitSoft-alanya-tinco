using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;

public class ObtenerCalificaciones
{
    private readonly ICalificacionRepository _repo;

    public ObtenerCalificaciones(ICalificacionRepository repo) => _repo = repo;

    public async Task<List<CalificacionDto>> EjecutarAsync(Guid profesionalId)
    {
        var lista = await _repo.ObtenerPorProfesionalAsync(profesionalId);
        return lista
            .Where(c => c.Visible)
            .OrderByDescending(c => c.FechaCreacion)
            .Select(c => new CalificacionDto(
                c.Id, c.NombreUsuario, c.Puntaje,
                c.Comentario, c.FechaCreacion, c.Reportada, c.MotivoReporte))
            .ToList();
    }
}
