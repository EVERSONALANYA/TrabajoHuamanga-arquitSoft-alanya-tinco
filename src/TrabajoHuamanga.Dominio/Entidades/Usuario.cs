namespace TrabajoHuamanga.Dominio.Entidades;

public enum RolUsuario
{
    Vecino,
    Profesional,
    Administrador
}

public class Usuario
{
    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public RolUsuario Rol { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public bool Activo { get; private set; } = true;
    private Usuario() { }
    public static Usuario Crear(string nombreCompleto, string email, string passwordHash, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new ArgumentException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("El email es obligatorio.");
        return new Usuario { Id = Guid.NewGuid(), NombreCompleto = nombreCompleto.Trim(), Email = email.Trim().ToLower(), PasswordHash = passwordHash, Rol = rol, FechaRegistro = DateTime.UtcNow, Activo = true };
    }
    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
