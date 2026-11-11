using Moq;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;
using Xunit;

namespace TrabajoHuamanga.Tests.Aplicacion;

public class BuscarProfesionalTests
{
    [Fact]
    public async Task Buscar_ConCache_RetornaDesdeCache()
    {
        var mockRepo = new Mock<IProfesionalRepository>();
        var mockCache = new Mock<ICacheBusqueda>();
        var profesionalesCacheados = new List<ProfesionalDto>
        {
            new(Guid.NewGuid(), "Juan", "966111111", "Desc", "Electricista", "Centro", null, 4.5, 10, true)
        };

        mockCache.Setup(c => c.ObtenerAsync<List<ProfesionalDto>>(It.IsAny<string>()))
            .ReturnsAsync(profesionalesCacheados);

        var caso = new BuscarProfesional(mockRepo.Object, mockCache.Object);
        var resultado = await caso.EjecutarAsync(new BusquedaDto(null, null, "Juan"));

        Assert.Single(resultado);
        Assert.Equal("Juan", resultado[0].NombreCompleto);
        mockRepo.Verify(r => r.BuscarPorRubroYZonaAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async Task Buscar_SinCache_ConsultaRepositorio()
    {
        var rubro = Rubro.Crear("Electricista", "Desc", "⚡");
        var zona = Zona.Crear("Centro", "Ayacucho");
        var profesional = Profesional.Crear("Pedro", "966222222", "Electricista", rubro.Id, zona.Id);

        var mockRepo = new Mock<IProfesionalRepository>();
        mockRepo.Setup(r => r.BuscarPorRubroYZonaAsync(null, null, "Pedro"))
            .ReturnsAsync(new List<Profesional> { profesional });

        var mockCache = new Mock<ICacheBusqueda>();
        mockCache.Setup(c => c.ObtenerAsync<List<ProfesionalDto>>(It.IsAny<string>()))
            .ReturnsAsync((List<ProfesionalDto>?)null);

        var caso = new BuscarProfesional(mockRepo.Object, mockCache.Object);
        var resultado = await caso.EjecutarAsync(new BusquedaDto(null, null, "Pedro"));

        Assert.Single(resultado);
        mockCache.Verify(c => c.GuardarAsync(It.IsAny<string>(), It.IsAny<List<ProfesionalDto>>(), It.IsAny<TimeSpan?>()), Times.Once);
    }
}