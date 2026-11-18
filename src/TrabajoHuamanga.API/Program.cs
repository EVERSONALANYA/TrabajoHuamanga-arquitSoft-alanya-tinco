using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Infraestructura.Datos;
using TrabajoHuamanga.Infraestructura.Repositorios;
using TrabajoHuamanga.Infraestructura.Adaptadores;
using TrabajoHuamanga.Infraestructura.Cache;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Contacto;
using TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;

var builder = WebApplication.CreateBuilder(args);

// ─── Capa Infraestructura: Base de datos ───
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// ─── Capa Infraestructura: Redis Cache ───
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TrabajoHuamanga_";
});

// ─── Capa Infraestructura: Repositorios (implementan interfaces del Dominio) ───
builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<IRubroRepository, RubroRepository>();
builder.Services.AddScoped<IZonaRepository, ZonaRepository>();

// ─── Capa Infraestructura: Adaptadores (implementan puertos del Dominio) ───
builder.Services.AddScoped<ICacheBusqueda, CacheBusquedaRedis>();
builder.Services.AddSingleton<IGeneradorEnlaceContacto, GeneradorEnlaceWhatsApp>();
builder.Services.AddSingleton<ISugeridorRubro>(sp =>
{
    var config = builder.Configuration.GetSection("OpenRouter");
    return new SugeridorRubroOpenRouter(
        new HttpClient(),
        config["ApiKey"] ?? "",
        config["Model"] ?? "google/gemini-flash-1.5");
});

// ─── Capa Aplicación: Casos de Uso ───
// Módulo Profesionales
builder.Services.AddScoped<RegistrarProfesional>();
builder.Services.AddScoped<ObtenerProfesional>();

// Módulo Búsqueda
builder.Services.AddScoped<BuscarProfesional>();

// Módulo Reputación
builder.Services.AddScoped<RegistrarCalificacion>();
builder.Services.AddScoped<ObtenerCalificaciones>();

// Módulo Contacto
builder.Services.AddScoped<GenerarEnlaceWhatsApp>();

// Módulo Sugerencia de Rubro (IA)
builder.Services.AddScoped<SugerirRubro>();

// Módulo Moderación
builder.Services.AddScoped<ModerarCalificacion>();
builder.Services.AddScoped<ReportarCalificacionCasoUso>();

// ─── Capa Presentación: Controllers + Swagger ───
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Trabajo Huamanga API", Version = "v1" });
});

// CORS para el frontend MVC
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Crear base de datos y aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();
