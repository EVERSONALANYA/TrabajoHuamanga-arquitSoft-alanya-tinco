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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Profesional
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

        // Calificacion
        modelBuilder.Entity<Calificacion>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.NombreUsuario).HasMaxLength(100).IsRequired();
            e.Property(c => c.Puntaje).IsRequired();
            e.Property(c => c.Comentario).HasMaxLength(500);
            e.Property(c => c.MotivoReporte).HasMaxLength(500);
        });

        // Rubro
        modelBuilder.Entity<Rubro>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Nombre).HasMaxLength(100).IsRequired();
            e.HasIndex(r => r.Nombre).IsUnique();
        });

        // Zona
        modelBuilder.Entity<Zona>(e =>
        {
            e.HasKey(z => z.Id);
            e.Property(z => z.Nombre).HasMaxLength(100).IsRequired();
        });

        // Datos semilla
        SembrarDatos(modelBuilder);
    }

    private static void SembrarDatos(ModelBuilder modelBuilder)
    {
        var rubros = new[]
        {
            Rubro.Crear("Electricista", "Instalaciones y reparaciones eléctricas", "⚡"),
            Rubro.Crear("Gasfitero", "Instalaciones y reparaciones de agua y desagüe", "🔧"),
            Rubro.Crear("Albañil", "Construcción y remodelación", "🧱"),
            Rubro.Crear("Pintor", "Pintura de interiores y exteriores", "🎨"),
            Rubro.Crear("Carpintero", "Muebles y estructuras de madera", "🪚"),
            Rubro.Crear("Cerrajero", "Chapas, llaves y seguridad", "🔑"),
            Rubro.Crear("Tutor académico", "Clases particulares y reforzamiento", "📚"),
            Rubro.Crear("Técnico en computadoras", "Reparación y mantenimiento de PC/laptops", "💻"),
            Rubro.Crear("Soldador", "Soldadura y estructuras metálicas", "🔥"),
            Rubro.Crear("Jardinero", "Mantenimiento de jardines y áreas verdes", "🌱"),
        };

        var zonas = new[]
        {
            Zona.Crear("Andrés Avelino Cáceres", "Ayacucho"),
            Zona.Crear("Carmen Alto", "Carmen Alto"),
            Zona.Crear("San Juan Bautista", "San Juan Bautista"),
            Zona.Crear("Jesús Nazareno", "Jesús Nazareno"),
            Zona.Crear("Centro Histórico", "Ayacucho"),
            Zona.Crear("Santa Ana", "Ayacucho"),
            Zona.Crear("La Libertad", "Ayacucho"),
            Zona.Crear("Conchopata", "Ayacucho"),
        };

        // EF Core seed requiere IDs fijos — los setea el factory method con Guid.NewGuid()
        // Para seed usamos reflexión para fijar IDs estables
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
    }

    private static Guid DeterministicGuid(string prefix, string name)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes($"{prefix}:{name}");
        using var md5 = System.Security.Cryptography.MD5.Create();
        return new Guid(md5.ComputeHash(bytes));
    }
}
