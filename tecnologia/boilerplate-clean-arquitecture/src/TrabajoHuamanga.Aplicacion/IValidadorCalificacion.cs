namespace TrabajoHuamanga.Aplicacion;

public interface IValidadorCalificacion
{
    Task<Resultado> ValidarAsync(RegistrarCalificacionComando comando);
}
