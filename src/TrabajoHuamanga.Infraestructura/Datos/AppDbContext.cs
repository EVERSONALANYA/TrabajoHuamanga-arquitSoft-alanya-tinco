using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio.Entidades;

namespace TrabajoHuamanga.Infraestructura.Datos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Profesional> Profesionales => Set<Profesional>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    public DbSet<Rubro> Rubros => Set<Rubro>();
    public DbSet<Zona> Zonas => Set<Zona>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profesional>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.NombreCompleto).HasMaxLength(200).IsRequired();
            e.Property(p => p.Telefono).HasMaxLength(20).IsRequired();
            e.Property(p => p.Descripcion).HasMaxLength(1000);
            e.Property(p => p.FotoUrl).HasMaxLength(500);
            e.HasOne(p => p.Rubro).WithMany().HasForeignKey(p => p.RubroId);
            e.HasOne(p => p.Zona).WithMany().HasForeignKey(p => p.ZonaId);
            e.HasMany(p => p.Calificaciones).WithOne(c => c.Profesional).HasForeignKey(c => c.ProfesionalId);
        });

        modelBuilder.Entity<Calificacion>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.NombreUsuario).HasMaxLength(100).IsRequired();
            e.Property(c => c.Puntaje).IsRequired();
            e.Property(c => c.Comentario).HasMaxLength(500);
            e.Property(c => c.MotivoReporte).HasMaxLength(500);
        });

        modelBuilder.Entity<Rubro>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Nombre).HasMaxLength(100).IsRequired();
            e.HasIndex(r => r.Nombre).IsUnique();
        });

        modelBuilder.Entity<Zona>(e =>
        {
            e.HasKey(z => z.Id);
            e.Property(z => z.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.NombreCompleto).HasMaxLength(200).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);
        });

        SembrarDatos(modelBuilder);
    }

    private static void SembrarDatos(ModelBuilder modelBuilder)
    {
        var rubros = new[]
        {
            Rubro.Crear("Electricista", "Instalaciones y reparaciones electricas", ""),
            Rubro.Crear("Gasfitero", "Instalaciones y reparaciones de agua y desague", ""),
            Rubro.Crear("Albanil", "Construccion y remodelacion", ""),
            Rubro.Crear("Pintor", "Pintura de interiores y exteriores", ""),
            Rubro.Crear("Carpintero", "Muebles y estructuras de madera", ""),
            Rubro.Crear("Cerrajero", "Chapas, llaves y seguridad", ""),
            Rubro.Crear("Tutor academico", "Clases particulares y reforzamiento", ""),
            Rubro.Crear("Tecnico en computadoras", "Reparacion y mantenimiento de PC/laptops", ""),
            Rubro.Crear("Soldador", "Soldadura y estructuras metalicas", ""),
            Rubro.Crear("Jardinero", "Mantenimiento de jardines y areas verdes", ""),
        };

        var zonas = new[]
        {
            Zona.Crear("Andres Avelino Caceres", "Ayacucho"),
            Zona.Crear("Carmen Alto", "Carmen Alto"),
            Zona.Crear("San Juan Bautista", "San Juan Bautista"),
            Zona.Crear("Jesus Nazareno", "Jesus Nazareno"),
            Zona.Crear("Centro Historico", "Ayacucho"),
            Zona.Crear("Santa Ana", "Ayacucho"),
            Zona.Crear("La Libertad", "Ayacucho"),
            Zona.Crear("Conchopata", "Ayacucho"),
        };

        modelBuilder.Entity<Rubro>().HasData(
            rubros.Select((r, i) => new { r.Id, r.Nombre, r.Descripcion, r.Icono, r.Activo }).ToArray()
                .Select(x => new { Id = DeterministicGuid("rubro", x.Nombre), x.Nombre, x.Descripcion, x.Icono, x.Activo })
                .ToArray()
        );

        modelBuilder.Entity<Zona>().HasData(
            zonas.Select((z, i) => new { z.Id, z.Nombre, z.Distrito, z.Activo }).ToArray()
                .Select(x => new { Id = DeterministicGuid("zona", x.Nombre), x.Nombre, x.Distrito, x.Activo })
                .ToArray()
        );

        var adminPasswordHash = BCryptHash("Admin123!");
        modelBuilder.Entity<Usuario>().HasData(new
        {
            Id = DeterministicGuid("usuario", "admin@trabajohuamanga.pe"),
            NombreCompleto = "Administrador",
            Email = "admin@trabajohuamanga.pe",
            PasswordHash = adminPasswordHash,
            Rol = RolUsuario.Administrador,
            FechaRegistro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Activo = true
        });
    }

    private static string BCryptHash(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "TrabajoHuamanga2024");
        var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static Guid DeterministicGuid(string prefix, string name)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(prefix + ":" + name);
        using var md5 = System.Security.Cryptography.MD5.Create();
        return new Guid(md5.ComputeHash(bytes));
    }
}
