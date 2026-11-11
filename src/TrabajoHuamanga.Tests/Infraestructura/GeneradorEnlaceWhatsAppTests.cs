using TrabajoHuamanga.Infraestructura.Adaptadores;
using Xunit;

namespace TrabajoHuamanga.Tests.Infraestructura;

public class GeneradorEnlaceWhatsAppTests
{
    [Fact]
    public void GenerarEnlace_ConTelefonoLocal_AgregaCodigoPeru()
    {
        var generador = new GeneradorEnlaceWhatsApp();
        var enlace = generador.GenerarEnlace("966123456", "Juan", "Electricista");

        Assert.StartsWith("https://wa.me/51966123456", enlace);
        Assert.Contains("Juan", enlace);
        Assert.Contains("Electricista", enlace);
    }

    [Fact]
    public void GenerarEnlace_ConCodigoPais_NoAgregaDuplicado()
    {
        var generador = new GeneradorEnlaceWhatsApp();
        var enlace = generador.GenerarEnlace("51966123456", "María", "Gasfitero");

        Assert.StartsWith("https://wa.me/51966123456", enlace);
        Assert.DoesNotContain("5151", enlace);
    }

    [Fact]
    public void GenerarEnlace_ConCaracteresEspeciales_LimpiaNumero()
    {
        var generador = new GeneradorEnlaceWhatsApp();
        var enlace = generador.GenerarEnlace("+51 966-123-456", "Pedro", "Albañil");

        Assert.StartsWith("https://wa.me/51966123456", enlace);
    }
}
