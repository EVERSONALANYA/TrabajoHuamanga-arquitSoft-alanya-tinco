namespace TrabajoHuamanga.Aplicacion.Validadores;

public class ValidadorNoRepetida : IValidadorCalificacion
{
    private readonly ICalificacionRepository _repositorio;

    public ValidadorNoRepetida(ICalificacionRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado> ValidarAsync(RegistrarCalificacionComando comando)
    {
        var yaExiste = await _repositorio.ExisteAsync(comando.VecinoId, comando.ProfesionalId);
        return yaExiste
            ? Resultado.Falla("El vecino ya calificó a este profesional.")
            : Resultado.Ok();
    }
}
