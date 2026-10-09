namespace TrabajoHuamanga.Aplicacion;

public record RegistrarCalificacionComando(Guid VecinoId, Guid ProfesionalId, int Puntaje, string Comentario);
