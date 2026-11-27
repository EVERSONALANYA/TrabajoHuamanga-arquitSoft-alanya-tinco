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

// PostgreSQL - convertir DATABASE_URL de Railway
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("PostgreSQL")!;
}

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

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Crear BD automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
