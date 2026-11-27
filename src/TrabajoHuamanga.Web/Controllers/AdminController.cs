using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly ModerarCalificacion _moderar;
    private readonly ObtenerProfesional _obtenerProfesional;
    private readonly ReportarCalificacionCasoUso _reportar;

    public AdminController(ModerarCalificacion moderar, ObtenerProfesional obtenerProfesional, ReportarCalificacionCasoUso reportar)
    { _moderar = moderar; _obtenerProfesional = obtenerProfesional; _reportar = reportar; }

    public async Task<IActionResult> Index()
    {
        var reportadas = await _moderar.ObtenerReportadasAsync();
        var profesionales = await _obtenerProfesional.ObtenerTodosAsync();
        ViewBag.TotalProfesionales = profesionales.Count;
        ViewBag.TotalReportadas = reportadas.Count;
        return View(reportadas);
    }

    [HttpPost]
    public async Task<IActionResult> Ocultar(Guid id)
    {
        try { await _moderar.OcultarAsync(id); TempData["Mensaje"] = "Calificacion ocultada"; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Restaurar(Guid id)
    {
        try { await _moderar.RestaurarAsync(id); TempData["Mensaje"] = "Calificacion restaurada"; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("Index");
    }
}
