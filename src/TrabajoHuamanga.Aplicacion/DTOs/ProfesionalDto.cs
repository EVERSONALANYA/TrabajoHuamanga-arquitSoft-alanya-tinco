namespace TrabajoHuamanga.Aplicacion.DTOs;

public record ProfesionalDto(
    Guid Id,
    string NombreCompleto,
    string Telefono,
    string Descripcion,
    string RubroNombre,
    string ZonaNombre,
    string? FotoUrl,
    double PromedioReputacion,
    int TotalCalificaciones,
    bool Activo
)
{
    public double PromedioCalificacion => PromedioReputacion;
};

public record CrearProfesionalDto(
    string NombreCompleto,
    string Telefono,
    string Descripcion,
    Guid RubroId,
    Guid ZonaId,
    string? FotoUrl
);

public record ActualizarProfesionalDto(
    string NombreCompleto,
    string Telefono,
    string Descripcion,
    Guid RubroId,
    Guid ZonaId
);

public record BusquedaDto(
    Guid? RubroId,
    Guid? ZonaId,
    string? Termino
);

public record CalificacionDto(
    Guid Id,
    string NombreUsuario,
    int Puntaje,
    string? Comentario,
    DateTime FechaCreacion,
    bool Reportada,
    string? MotivoReporte = null
);

public record CrearCalificacionDto(
    Guid ProfesionalId,
    string NombreUsuario,
    int Puntaje,
    string? Comentario
);

public record ReportarCalificacionDto(
    Guid CalificacionId,
    string Motivo
);

public record RubroDto(Guid Id, string Nombre, string? Descripcion, string? Icono);
public record ZonaDto(Guid Id, string Nombre, string? Distrito);
public record SugerenciaRubroDto(string Descripcion);
public record EnlaceContactoDto(string Url, string NombreProfesional);
