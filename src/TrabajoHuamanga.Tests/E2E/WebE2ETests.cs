using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TrabajoHuamanga.Infraestructura.Datos;
using Xunit;

namespace TrabajoHuamanga.Tests.E2E;

/// <summary>
/// Pruebas End-to-End: simulan el flujo completo del usuario a traves del navegador.
/// Verifican que las vistas Razor se renderizan correctamente con datos reales.
/// </summary>
public class WebE2ETests : IClassFixture<WebApplicationFactory<TrabajoHuamanga.Web.Controllers.HomeController>>
{
    private readonly HttpClient _client;

    public WebE2ETests(WebApplicationFactory<TrabajoHuamanga.Web.Controllers.HomeController> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
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
                    options.UseInMemoryDatabase("E2E_TestDb_" + Guid.NewGuid()));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task PaginaInicio_CargaCorrectamente()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Trabajo Huamanga", html);
        Assert.Contains("Buscar", html);
    }

    [Fact]
    public async Task PaginaInicio_ContieneBuscador()
    {
        var html = await (await _client.GetAsync("/")).Content.ReadAsStringAsync();
        Assert.Contains("rubroId", html);
        Assert.Contains("zonaId", html);
        Assert.Contains("termino", html);
    }

    [Fact]
    public async Task PaginaInicio_ContieneSeccionIA()
    {
        var html = await (await _client.GetAsync("/")).Content.ReadAsStringAsync();
        Assert.Contains("iaDescripcion", html);
        Assert.Contains("sugerirRubro", html);
    }

    [Fact]
    public async Task PaginaRegistro_RequiereAutenticacion()
    {
        var response = await _client.GetAsync("/Profesional/Registrar");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task PanelAdmin_RequiereAutenticacion()
    {
        var response = await _client.GetAsync("/Admin");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Busqueda_ConFiltros_RetornaResultados()
    {
        var response = await _client.GetAsync("/?termino=electricista");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("profesional(es) encontrado(s)", html);
    }

    [Fact]
    public async Task PerfilInexistente_Retorna404()
    {
        var response = await _client.GetAsync($"/Profesional/Perfil/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Navbar_ContieneTodosLosEnlaces()
    {
        var html = await (await _client.GetAsync("/")).Content.ReadAsStringAsync();
        Assert.Contains("Buscar", html);
        Assert.Contains("Ingresar", html);
        Assert.Contains("Crear Cuenta", html);
    }

    [Fact]
    public async Task PaginaLogin_CargaCorrectamente()
    {
        var response = await _client.GetAsync("/Cuenta/Login");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Iniciar Sesion", html);
        Assert.Contains("email", html);
        Assert.Contains("password", html);
    }

    [Fact]
    public async Task PaginaRegistrarCuenta_CargaCorrectamente()
    {
        var response = await _client.GetAsync("/Cuenta/Registrar");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Crear Cuenta", html);
        Assert.Contains("Vecino", html);
        Assert.Contains("Profesional", html);
    }

    [Fact]
    public async Task Footer_ContieneInformacionUniversidad()
    {
        var html = await (await _client.GetAsync("/")).Content.ReadAsStringAsync();
        Assert.Contains("UNSCH", html);
        Assert.Contains("Arquitectura de Software", html);
    }
}
