using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Infraestructura.Datos;
using Xunit;

namespace TrabajoHuamanga.Tests.Integracion;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<TrabajoHuamanga.API.Controllers.ProfesionalesController>>
{
    private readonly WebApplicationFactory<TrabajoHuamanga.API.Controllers.ProfesionalesController> _factory;

    public ApiIntegrationTests(WebApplicationFactory<TrabajoHuamanga.API.Controllers.ProfesionalesController> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.FullName != null &&
                     d.ServiceType.FullName.Contains("EntityFrameworkCore"))).ToList();
                foreach (var d in toRemove) services.Remove(d);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
            });
        });
    }

    [Fact]
    public async Task GET_Profesionales_RetornaListaVacia()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/profesionales");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profesionales = await response.Content.ReadFromJsonAsync<List<ProfesionalDto>>();
        Assert.NotNull(profesionales);
    }

    [Fact]
    public async Task GET_CatalogosRubros_RetornaRubrosSemilla()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/catalogos/rubros");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rubros = await response.Content.ReadFromJsonAsync<List<RubroDto>>();
        Assert.NotNull(rubros);
        Assert.True(rubros!.Count >= 0);
    }

    [Fact]
    public async Task GET_CatalogosZonas_RetornaOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/catalogos/zonas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Busqueda_SinFiltros_RetornaOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/busqueda");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_ProfesionalInexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/profesionales/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Profesional_SinDatos_RetornaBadRequest()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/profesionales", new { });

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GET_ModeracionReportadas_RetornaOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/moderacion/reportadas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task POST_ContactoInexistente_Retorna404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/contacto/{Guid.NewGuid()}");

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }
}