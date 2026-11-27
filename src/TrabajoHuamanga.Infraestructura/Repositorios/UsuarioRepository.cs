using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Infraestructura.Datos;

namespace TrabajoHuamanga.Infraestructura.Repositorios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;
    public async Task<Usuario?> ObtenerPorEmailAsync(string email) => await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower());
    public async Task<Usuario?> ObtenerPorIdAsync(Guid id) => await _db.Usuarios.FindAsync(id);
    public async Task<List<Usuario>> ObtenerTodosAsync() => await _db.Usuarios.OrderByDescending(u => u.FechaRegistro).ToListAsync();
    public async Task AgregarAsync(Usuario usuario) { _db.Usuarios.Add(usuario); await _db.SaveChangesAsync(); }
    public async Task<bool> ExisteEmailAsync(string email) => await _db.Usuarios.AnyAsync(u => u.Email == email.ToLower());
}
