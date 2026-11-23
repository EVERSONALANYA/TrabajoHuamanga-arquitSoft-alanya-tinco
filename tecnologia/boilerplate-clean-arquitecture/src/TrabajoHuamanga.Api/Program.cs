using TrabajoHuamanga.Dominio;
using TrabajoHuamanga.Aplicacion;
using TrabajoHuamanga.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICalificacionRepository, CalificacionRepositoryEnMemoria>();
builder.Services.AddTransient<IValidadorCalificacion, TrabajoHuamanga.Aplicacion.Validadores.ValidadorPuntaje>();
builder.Services.AddTransient<IValidadorCalificacion, TrabajoHuamanga.Aplicacion.Validadores.ValidadorNoRepetida>();
builder.Services.AddTransient<RegistrarCalificacion>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Trabajo Huamanga API", Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var profesionales = new List<Profesional>
{
    new() { Id = Guid.NewGuid(), Nombre = "Juan Perez", Rubro = "Electricista", Distrito = "Ayacucho" },
    new() { Id = Guid.NewGuid(), Nombre = "Maria Lopez", Rubro = "Gasfitero", Distrito = "San Juan Bautista" },
    new() { Id = Guid.NewGuid(), Nombre = "Carlos Quispe", Rubro = "Tutor de Matematicas", Distrito = "Carmen Alto" },
    new() { Id = Guid.NewGuid(), Nombre = "Ana Huaman", Rubro = "Electricista", Distrito = "Jesus Nazareno" },
    new() { Id = Guid.NewGuid(), Nombre = "Pedro Flores", Rubro = "Pintor", Distrito = "Andres Avelino Caceres" }
};

var rubros = new[] { "Electricista", "Gasfitero", "Pintor", "Carpintero", "Tutor de Matematicas", "Tutor de Ingles", "Mecanico", "Albanil" };
var zonas = new[] { "Ayacucho", "San Juan Bautista", "Carmen Alto", "Jesus Nazareno", "Andres Avelino Caceres" };

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/api/rubros", () => Results.Ok(rubros));
app.MapGet("/api/zonas", () => Results.Ok(zonas));
app.MapGet("/api/profesionales", () => Results.Ok(profesionales));

app.MapGet("/api/profesionales/{id}", (Guid id) =>
{
    var p = profesionales.FirstOrDefault(x => x.Id == id);
    if (p is null) return Results.NotFound(new { error = "Profesional no encontrado" });
    return Results.Ok(p);
});

app.MapGet("/api/profesionales/buscar", (string? rubro, string? distrito) =>
{
    var resultado = profesionales.AsEnumerable();
    if (!string.IsNullOrEmpty(rubro))
        resultado = resultado.Where(p => p.Rubro.Contains(rubro, StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrEmpty(distrito))
        resultado = resultado.Where(p => p.Distrito.Contains(distrito, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(resultado);
});

app.MapPost("/api/profesionales", (Profesional nuevo) =>
{
    nuevo.Id = Guid.NewGuid();
    profesionales.Add(nuevo);
    return Results.Created("/api/profesionales/" + nuevo.Id, nuevo);
});

app.MapPut("/api/profesionales/{id}", (Guid id, Profesional actualizado) =>
{
    var p = profesionales.FirstOrDefault(x => x.Id == id);
    if (p is null) return Results.NotFound(new { error = "Profesional no encontrado" });
    p.Nombre = actualizado.Nombre;
    p.Rubro = actualizado.Rubro;
    p.Distrito = actualizado.Distrito;
    return Results.Ok(p);
});

app.MapPost("/api/calificaciones", async (RegistrarCalificacionComando cmd, RegistrarCalificacion uc) =>
{
    var r = await uc.EjecutarAsync(cmd);
    if (!r.Exito)
    {
        if (r.Error!.Contains("duplicada") || r.Error.Contains("ya existe"))
            return Results.Conflict(new { error = r.Error });
        return Results.BadRequest(new { error = r.Error });
    }
    return Results.Created("/api/calificaciones", new { mensaje = "Calificacion registrada" });
});

app.MapGet("/api/calificaciones/profesional/{pid}", async (Guid pid, ICalificacionRepository repo) =>
{
    var cal = await repo.ObtenerPorProfesionalAsync(pid);
    return Results.Ok(cal);
});

app.MapGet("/api/reputacion/{pid}", async (Guid pid, ICalificacionRepository repo) =>
{
    var cal = await repo.ObtenerPorProfesionalAsync(pid);
    var prom = Profesional.CalcularReputacion(cal);
    return Results.Ok(new { profesionalId = pid, promedio = prom, total = cal.Count(c => !c.Oculta) });
});

app.MapPatch("/api/calificaciones/{id}/ocultar", (Guid id) =>
{
    return Results.Ok(new { mensaje = "Calificacion ocultada", id });
});

var port = Environment.GetEnvironmentVariable("PORT");
if (string.IsNullOrEmpty(port)) port = "8080";
app.Run("http://0.0.0.0:" + port);
