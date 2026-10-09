using TrabajoHuamanga.Aplicacion;
using TrabajoHuamanga.Aplicacion.Validadores;
using TrabajoHuamanga.Dominio;
using TrabajoHuamanga.Infraestructura;
using Xunit;

namespace TrabajoHuamanga.Pruebas;

public class RegistrarCalificacionPruebas
{
    private static (RegistrarCalificacion casoDeUso, CalificacionRepositoryEnMemoria repo) Crear()
    {
        var repo = new CalificacionRepositoryEnMemoria();
        var validadores = new IValidadorCalificacion[]
        {
            new ValidadorPuntaje(),
            new ValidadorNoRepetida(repo)
        };
        return (new RegistrarCalificacion(repo, validadores), repo);
    }

    [Fact]
    public async Task CalificacionValida_SeGuarda()
    {
        var (casoDeUso, repo) = Crear();
        var vecino = Guid.NewGuid();
        var profesional = Guid.NewGuid();

        var resultado = await casoDeUso.EjecutarAsync(new(vecino, profesional, 5, "Muy puntual"));

        Assert.True(resultado.Exito);
        Assert.True(await repo.ExisteAsync(vecino, profesional));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task PuntajeFueraDeRango_SeRechaza(int puntaje)
    {
        var (casoDeUso, _) = Crear();

        var resultado = await casoDeUso.EjecutarAsync(new(Guid.NewGuid(), Guid.NewGuid(), puntaje, ""));

        Assert.False(resultado.Exito);
    }

    [Fact]
    public async Task CalificacionRepetida_SeRechaza()
    {
        var (casoDeUso, _) = Crear();
        var vecino = Guid.NewGuid();
        var profesional = Guid.NewGuid();
        await casoDeUso.EjecutarAsync(new(vecino, profesional, 4, "Bien"));

        var segunda = await casoDeUso.EjecutarAsync(new(vecino, profesional, 5, "Otra vez"));

        Assert.False(segunda.Exito);
    }

    [Fact]
    public void Reputacion_IgnoraCalificacionesOcultas()
    {
        var profesional = Guid.NewGuid();
        var buena = new Calificacion(Guid.NewGuid(), profesional, 5, "");
        var mala = new Calificacion(Guid.NewGuid(), profesional, 1, "");
        mala.Ocultar();

        var promedio = Profesional.CalcularReputacion(new[] { buena, mala });

        Assert.Equal(5m, promedio);
    }
}
