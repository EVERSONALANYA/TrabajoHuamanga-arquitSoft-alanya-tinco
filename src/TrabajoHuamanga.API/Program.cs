using Microsoft.EntityFrameworkCore;
using TrabajoHuamanga.Dominio;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Infraestructura.Datos;
using TrabajoHuamanga.Infraestructura.Repositorios;
using TrabajoHuamanga.Infraestructura.Cache;
using TrabajoHuamanga.Infraestructura.Adaptadores;
using TrabajoHuamanga.Aplicacion;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<IRubroRepository, RubroRepository>();
builder.Services.AddScoped<IZonaRepository, ZonaRepository>();

builder.Services.AddScoped<ICacheBusqueda, CacheBusquedaMemoria>();
builder.Services.AddSingleton<IGeneradorEnlaceContacto, GeneradorEnlaceWhatsApp>();
builder.Services.AddSingleton<ISugerIdorRubro>(sp => { var config = builder.Configuration.GetSection("OpenRouter"); return new SugerIdorRubroOpenRouter(new HttpClient(), config["ApiKey"] ?? "", config["Model"] ?? "google/gemini-flash-1.5"); });

builder.Services.AddScoped<RegistrarProfesional>();
builder.Services.AddScoped<ObtenerProfesional>();
builder.Services.AddScoped<BuscarProfesional>();
builder.Services.AddScoped<RegistrarCalificacion>();
builder.Services.AddScoped<ObtenerCalificaciones>();
builder.Services.AddScoped<GenerarEnlaceContacto>();
builder.Services.AddScoped<SugerirRubro>();
builder.Services.AddScoped<ModerarCalificacion>();
builder.Services.AddScoped<ReportarCalificacionCasoUso>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "Trabajo Huamanga API", Version = "v1" }); });

builder.Services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });

var app = builder.Build();

using (var scope = app.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.Database.EnsureCreated(); }

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run("http://0.0.0.0:" + port);
