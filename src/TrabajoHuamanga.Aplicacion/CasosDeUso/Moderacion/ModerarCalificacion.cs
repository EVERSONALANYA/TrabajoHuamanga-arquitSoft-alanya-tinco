using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;

public class ModerarCalificacion
{
    private readonly ICalificacionRepository _calRepo;

    public ModerarCalificacion(ICalificacionRepository calRepo) => _calRepo = calRepo;

    public async Task<List<CalificacionDto>> ObtenerReportadasAsync()
    {
        var lista = await _calRepo.ObtenerReportadasAsync();
        return lista.Select(c => new CalificacionDto(
            c.Id, c.NombreUsuario, c.Puntaje,
            c.Comentario, c.FechaCreacion, c.Reportada, c.MotivoReporte))
            .ToList();
    }

    public async Task OcultarAsync(Guid calificacionId)
    {
        var cal = await _calRepo.ObtenerPorIdAsync(calificacionId)
            ?? throw new ArgumentException("Calificación no encontrada.");
        cal.OcultarPorModeracion();
        await _calRepo.ActualizarAsync(cal);
    }

    public async Task RestaurarAsync(Guid calificacionId)
    {
        var cal = await _calRepo.ObtenerPorIdAsync(calificacionId)
            ?? throw new ArgumentException("Calificación no encontrada.");
        cal.Restaurar();
        await _calRepo.ActualizarAsync(cal);
    }
}

public class ReportarCalificacionCasoUso
{
    private readonly ICalificacionRepository _repo;

    public ReportarCalificacionCasoUso(ICalificacionRepository repo) => _repo = repo;

    public async Task EjecutarAsync(ReportarCalificacionDto dto)
    {
        var cal = await _repo.ObtenerPorIdAsync(dto.CalificacionId)
            ?? throw new ArgumentException("Calificación no encontrada.");
        cal.Reportar(dto.Motivo);
        await _repo.ActualizarAsync(cal);
    }
}
