using TrabajoHuamanga.Dominio.Entidades;

namespace TrabajoHuamanga.Dominio.Interfaces;

public interface IProfesionalRepository
{
    Task<Profesional?> ObtenerPorIdAsync(Guid id);
    Task<List<Profesional>> ObtenerTodosAsync();
    Task<List<Profesional>> BuscarPorRubroYZonaAsync(Guid? rubroId, Guid? zonaId, string? termino);
    Task AgregarAsync(Profesional profesional);
    Task ActualizarAsync(Profesional profesional);
    Task<bool> ExisteAsync(Guid id);
}

public interface ICalificacionRepository
{
    Task<List<Calificacion>> ObtenerPorProfesionalAsync(Guid profesionalId);
    Task<List<Calificacion>> ObtenerReportadasAsync();
    Task AgregarAsync(Calificacion calificacion);
    Task ActualizarAsync(Calificacion calificacion);
    Task<Calificacion?> ObtenerPorIdAsync(Guid id);
    Task<bool> UsuarioYaCalificoAsync(Guid profesionalId, string nombreUsuario);
}

public interface IRubroRepository
{
    Task<List<Rubro>> ObtenerTodosAsync();
    Task<Rubro?> ObtenerPorIdAsync(Guid id);
    Task<Rubro?> ObtenerPorNombreAsync(string nombre);
    Task AgregarAsync(Rubro rubro);
}

public interface IZonaRepository
{
    Task<List<Zona>> ObtenerTodosAsync();
    Task<Zona?> ObtenerPorIdAsync(Guid id);
    Task AgregarAsync(Zona zona);
}
