using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.Web.Controllers;

/// <summary>
/// Panel de Administración — Moderación de calificaciones y gestión.
/// </summary>
public class AdminController : Controller
{
    private readonly ModerarCalificacion _moderar;
    private readonly ObtenerProfesional _obtenerProfesional;
    private readonly ReportarCalificacionCasoUso _reportar;

    public AdminController(
        ModerarCalificacion moderar,
        ObtenerProfesional obtenerProfesional,
        ReportarCalificacionCasoUso reportar)
    {
        _moderar = moderar;
        _obtenerProfesional = obtenerProfesional;
        _reportar = reportar;
    }

    /// <summary>Panel principal del administrador</summary>
    public async Task<IActionResult> Index()
    {
        var reportadas = await _moderar.ObtenerReportadasAsync();
        var profesionales = await _obtenerProfesional.ObtenerTodosAsync();
        ViewBag.TotalProfesionales = profesionales.Count;
        ViewBag.TotalReportadas = reportadas.Count;
        return View(reportadas);
    }

    /// <summary>Ocultar calificación reportada</summary>
    [HttpPost]
    public async Task<IActionResult> Ocultar(Guid id)
    {
        try
        {
            await _moderar.OcultarAsync(id);
            TempData["Mensaje"] = "Calificación ocultada";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }

    /// <summary>Restaurar calificación ocultada</summary>
    [HttpPost]
    public async Task<IActionResult> Restaurar(Guid id)
    {
        try
        {
            await _moderar.RestaurarAsync(id);
            TempData["Mensaje"] = "Calificación restaurada";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }
}
