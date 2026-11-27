using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Infraestructura.Datos;
using TrabajoHuamanga.Infraestructura.Repositorios;
using TrabajoHuamanga.Infraestructura.Cache;
using TrabajoHuamanga.Infraestructura.Adaptadores;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Contacto;
using TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// IMemoryCache
builder.Services.AddMemoryCache();

// Repositorios
builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<IRubroRepository, RubroRepository>();
builder.Services.AddScoped<IZonaRepository, ZonaRepository>();

// Adaptadores
builder.Services.AddScoped<ICacheBusqueda, CacheBusquedaMemoria>();
builder.Services.AddSingleton<IGeneradorEnlaceContacto, GeneradorEnlaceWhatsApp>();
builder.Services.AddSingleton<ISugeridorRubro>(sp =>
{
    var config = builder.Configuration.GetSection("OpenRouter");
    return new SugeridorRubroOpenRouter(
        new HttpClient(),
        config["ApiKey"] ?? "",
        config["Model"] ?? "google/gemini-flash-1.5");
});

// Casos de Uso
builder.Services.AddScoped<RegistrarProfesional>();
builder.Services.AddScoped<ObtenerProfesional>();
builder.Services.AddScoped<BuscarProfesional>();
builder.Services.AddScoped<RegistrarCalificacion>();
builder.Services.AddScoped<ObtenerCalificaciones>();
builder.Services.AddScoped<GenerarEnlaceWhatsApp>();
builder.Services.AddScoped<SugerirRubro>();
builder.Services.AddScoped<ModerarCalificacion>();
builder.Services.AddScoped<ReportarCalificacionCasoUso>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Trabajo Huamanga API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
