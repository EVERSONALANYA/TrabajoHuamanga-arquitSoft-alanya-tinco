using TrabajoHuamanga.Dominio;

namespace TrabajoHuamanga.Aplicacion;

// Puerto: lo define la capa de Aplicacion y lo implementa Infraestructura.
public interface ICalificacionRepository
{
    Task<bool> ExisteAsync(Guid vecinoId, Guid profesionalId);
    Task GuardarAsync(Calificacion calificacion);
    Task<List<Calificacion>> ObtenerPorProfesionalAsync(Guid profesionalId);
}
