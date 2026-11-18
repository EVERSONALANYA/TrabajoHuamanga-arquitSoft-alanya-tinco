using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;

public class RegistrarCalificacion
{
    private readonly ICalificacionRepository _calRepo;
    private readonly IProfesionalRepository _profRepo;
    private readonly ICacheBusqueda _cache;

    public RegistrarCalificacion(
        ICalificacionRepository calRepo,
        IProfesionalRepository profRepo,
        ICacheBusqueda cache)
    {
        _calRepo = calRepo;
        _profRepo = profRepo;
        _cache = cache;
    }

    public async Task<CalificacionDto> EjecutarAsync(CrearCalificacionDto dto)
    {
        // Validar que el profesional existe
        if (!await _profRepo.ExisteAsync(dto.ProfesionalId))
            throw new ArgumentException("El profesional no existe.");

        // Validar que el usuario no haya calificado ya
        if (await _calRepo.UsuarioYaCalificoAsync(dto.ProfesionalId, dto.NombreUsuario))
            throw new InvalidOperationException("Ya calificaste a este profesional.");

        var calificacion = Calificacion.Crear(
            dto.ProfesionalId, dto.NombreUsuario, dto.Puntaje, dto.Comentario);

        await _calRepo.AgregarAsync(calificacion);

        // Invalidar caché de búsquedas (el promedio cambió)
        await _cache.EliminarAsync($"busqueda:*");

        return new CalificacionDto(
            calificacion.Id, calificacion.NombreUsuario, calificacion.Puntaje,
            calificacion.Comentario, calificacion.FechaCreacion, false);
    }
}
