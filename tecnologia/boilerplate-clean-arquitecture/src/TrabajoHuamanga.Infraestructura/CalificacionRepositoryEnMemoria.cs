using TrabajoHuamanga.Aplicacion;
using TrabajoHuamanga.Dominio;

namespace TrabajoHuamanga.Infraestructura;

// Implementacion de ejemplo. En el sistema real se reemplaza por una
// implementacion con EF Core y SQL Server sin tocar Dominio ni Aplicacion.
public class CalificacionRepositoryEnMemoria : ICalificacionRepository
{
    private readonly List<Calificacion> _datos = new();

    public Task<bool> ExisteAsync(Guid vecinoId, Guid profesionalId) =>
        Task.FromResult(_datos.Any(c => c.VecinoId == vecinoId && c.ProfesionalId == profesionalId));

    public Task GuardarAsync(Calificacion calificacion)
    {
        _datos.Add(calificacion);
        return Task.CompletedTask;
    }

    public Task<List<Calificacion>> ObtenerPorProfesionalAsync(Guid profesionalId) =>
        Task.FromResult(_datos.Where(c => c.ProfesionalId == profesionalId).ToList());
}
