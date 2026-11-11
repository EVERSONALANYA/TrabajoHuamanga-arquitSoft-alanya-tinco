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

// ─── Infraestructura ───
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TrabajoHuamanga_";
});

// ─── Repositorios ───
builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<IRubroRepository, RubroRepository>();
builder.Services.AddScoped<IZonaRepository, ZonaRepository>();

// ─── Adaptadores ───
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

// ─── Casos de Uso ───
builder.Services.AddScoped<RegistrarProfesional>();
builder.Services.AddScoped<ObtenerProfesional>();
builder.Services.AddScoped<BuscarProfesional>();
builder.Services.AddScoped<RegistrarCalificacion>();
builder.Services.AddScoped<ObtenerCalificaciones>();
builder.Services.AddScoped<GenerarEnlaceWhatsApp>();
builder.Services.AddScoped<SugerirRubro>();
builder.Services.AddScoped<ModerarCalificacion>();
builder.Services.AddScoped<ReportarCalificacionCasoUso>();

// ─── MVC ───
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Crear BD automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
