using TrabajoHuamanga.Dominio;

namespace TrabajoHuamanga.Aplicacion;

// Caso de uso: cadena de validadores y luego guardado.
public class RegistrarCalificacion
{
    private readonly ICalificacionRepository _repositorio;
    private readonly IEnumerable<IValidadorCalificacion> _validadores;

    public RegistrarCalificacion(
        ICalificacionRepository repositorio,
        IEnumerable<IValidadorCalificacion> validadores)
    {
        _repositorio = repositorio;
        _validadores = validadores;
    }

    public async Task<Resultado> EjecutarAsync(RegistrarCalificacionComando comando)
    {
        foreach (var validador in _validadores)
        {
            var resultado = await validador.ValidarAsync(comando);
            if (!resultado.Exito) return resultado;
        }

        var calificacion = new Calificacion(
            comando.VecinoId, comando.ProfesionalId, comando.Puntaje, comando.Comentario);

        await _repositorio.GuardarAsync(calificacion);
        return Resultado.Ok();
    }
}
