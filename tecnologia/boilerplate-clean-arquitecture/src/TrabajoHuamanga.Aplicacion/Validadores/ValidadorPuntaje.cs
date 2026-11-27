namespace TrabajoHuamanga.Aplicacion.Validadores;

public class ValidadorPuntaje : IValidadorCalificacion
{
    public Task<Resultado> ValidarAsync(RegistrarCalificacionComando comando)
    {
        var resultado = comando.Puntaje is >= 1 and <= 5
            ? Resultado.Ok()
            : Resultado.Falla("El puntaje debe estar entre 1 y 5.");
        return Task.FromResult(resultado);
    }
}
