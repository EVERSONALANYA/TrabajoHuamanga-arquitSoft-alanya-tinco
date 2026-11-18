using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;
using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Web.Controllers;

/// <summary>
/// Controlador principal — Vista de búsqueda para el Usuario.
/// </summary>
public class HomeController : Controller
{
    private readonly BuscarProfesional _buscar;
    private readonly SugerirRubro _sugerir;
    private readonly IRubroRepository _rubros;
    private readonly IZonaRepository _zonas;

    public HomeController(
        BuscarProfesional buscar,
        SugerirRubro sugerir,
        IRubroRepository rubros,
        IZonaRepository zonas)
    {
        _buscar = buscar;
        _sugerir = sugerir;
        _rubros = rubros;
        _zonas = zonas;
    }

    public async Task<IActionResult> Index(Guid? rubroId, Guid? zonaId, string? termino)
    {
        ViewBag.Rubros = await _rubros.ObtenerTodosAsync();
        ViewBag.Zonas = await _zonas.ObtenerTodosAsync();
        ViewBag.RubroSeleccionado = rubroId;
        ViewBag.ZonaSeleccionada = zonaId;
        ViewBag.Termino = termino;

        var dto = new BusquedaDto(rubroId, zonaId, termino);
        var resultados = await _buscar.EjecutarAsync(dto);
        return View(resultados);
    }

    [HttpPost]
    public async Task<IActionResult> SugerirRubro(string descripcion)
    {
        try
        {
            var sugerencia = await _sugerir.EjecutarAsync(new SugerenciaRubroDto(descripcion));
            return Json(sugerencia);
        }
        catch
        {
            return Json(new { error = "No se pudo obtener sugerencia" });
        }
    }

    public IActionResult Error() => View();
}
