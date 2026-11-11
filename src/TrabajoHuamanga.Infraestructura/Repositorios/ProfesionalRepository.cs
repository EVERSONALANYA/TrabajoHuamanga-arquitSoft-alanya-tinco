using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Infraestructura.Datos;

namespace TrabajoHuamanga.Infraestructura.Repositorios;

public class ProfesionalRepository : IProfesionalRepository
{
    private readonly AppDbContext _db;
    public ProfesionalRepository(AppDbContext db) => _db = db;

    public async Task<Profesional?> ObtenerPorIdAsync(Guid id) =>
        await _db.Profesionales
            .Include(p => p.Rubro)
            .Include(p => p.Zona)
            .Include(p => p.Calificaciones)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Profesional>> ObtenerTodosAsync() =>
        await _db.Profesionales
            .Include(p => p.Rubro)
            .Include(p => p.Zona)
            .Include(p => p.Calificaciones)
            .Where(p => p.Activo)
            .ToListAsync();

    public async Task<List<Profesional>> BuscarPorRubroYZonaAsync(Guid? rubroId, Guid? zonaId, string? termino)
    {
        var query = _db.Profesionales
            .Include(p => p.Rubro)
            .Include(p => p.Zona)
            .Include(p => p.Calificaciones)
            .Where(p => p.Activo)
            .AsQueryable();

        if (rubroId.HasValue && rubroId != Guid.Empty)
            query = query.Where(p => p.RubroId == rubroId.Value);

        if (zonaId.HasValue && zonaId != Guid.Empty)
            query = query.Where(p => p.ZonaId == zonaId.Value);

        if (!string.IsNullOrWhiteSpace(termino))
            query = query.Where(p =>
                p.NombreCompleto.Contains(termino) ||
                p.Descripcion.Contains(termino));

        return await query.ToListAsync();
    }

    public async Task AgregarAsync(Profesional profesional)
    {
        _db.Profesionales.Add(profesional);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Profesional profesional)
    {
        _db.Profesionales.Update(profesional);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExisteAsync(Guid id) =>
        await _db.Profesionales.AnyAsync(p => p.Id == id);
}

public class CalificacionRepository : ICalificacionRepository
{
    private readonly AppDbContext _db;
    public CalificacionRepository(AppDbContext db) => _db = db;

    public async Task<List<Calificacion>> ObtenerPorProfesionalAsync(Guid profesionalId) =>
        await _db.Calificaciones
            .Where(c => c.ProfesionalId == profesionalId)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync();

    public async Task<List<Calificacion>> ObtenerReportadasAsync() =>
        await _db.Calificaciones
            .Include(c => c.Profesional)
            .Where(c => c.Reportada && c.Visible)
            .ToListAsync();

    public async Task AgregarAsync(Calificacion calificacion)
    {
        _db.Calificaciones.Add(calificacion);
        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Calificacion calificacion)
    {
        _db.Calificaciones.Update(calificacion);
        await _db.SaveChangesAsync();
    }

    public async Task<Calificacion?> ObtenerPorIdAsync(Guid id) =>
        await _db.Calificaciones.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> UsuarioYaCalificoAsync(Guid profesionalId, string nombreUsuario) =>
        await _db.Calificaciones.AnyAsync(c =>
            c.ProfesionalId == profesionalId &&
            c.NombreUsuario == nombreUsuario);
}

public class RubroRepository : IRubroRepository
{
    private readonly AppDbContext _db;
    public RubroRepository(AppDbContext db) => _db = db;

    public async Task<List<Rubro>> ObtenerTodosAsync() =>
        await _db.Rubros.Where(r => r.Activo).OrderBy(r => r.Nombre).ToListAsync();

    public async Task<Rubro?> ObtenerPorIdAsync(Guid id) =>
        await _db.Rubros.FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Rubro?> ObtenerPorNombreAsync(string nombre) =>
        await _db.Rubros.FirstOrDefaultAsync(r =>
            r.Nombre.ToLower().Contains(nombre.ToLower()));

    public async Task AgregarAsync(Rubro rubro)
    {
        _db.Rubros.Add(rubro);
        await _db.SaveChangesAsync();
    }
}

public class ZonaRepository : IZonaRepository
{
    private readonly AppDbContext _db;
    public ZonaRepository(AppDbContext db) => _db = db;

    public async Task<List<Zona>> ObtenerTodosAsync() =>
        await _db.Zonas.Where(z => z.Activo).OrderBy(z => z.Nombre).ToListAsync();

    public async Task<Zona?> ObtenerPorIdAsync(Guid id) =>
        await _db.Zonas.FirstOrDefaultAsync(z => z.Id == id);

    public async Task AgregarAsync(Zona zona)
    {
        _db.Zonas.Add(zona);
        await _db.SaveChangesAsync();
    }
}
