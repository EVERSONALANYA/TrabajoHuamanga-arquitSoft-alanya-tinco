using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Contacto;
using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Web.Controllers;

public class ProfesionalController : Controller
{
    private readonly ObtenerProfesional _obtener;
    private readonly RegistrarProfesional _registrar;
    private readonly ObtenerCalificaciones _calificaciones;
    private readonly RegistrarCalificacion _registrarCalificacion;
    private readonly GenerarEnlaceWhatsApp _enlaceWhatsApp;
    private readonly IRubroRepository _rubros;
    private readonly IZonaRepository _zonas;

    public ProfesionalController(ObtenerProfesional obtener, RegistrarProfesional registrar, ObtenerCalificaciones calificaciones, RegistrarCalificacion registrarCalificacion, GenerarEnlaceWhatsApp enlaceWhatsApp, IRubroRepository rubros, IZonaRepository zonas)
    { _obtener = obtener; _registrar = registrar; _calificaciones = calificaciones; _registrarCalificacion = registrarCalificacion; _enlaceWhatsApp = enlaceWhatsApp; _rubros = rubros; _zonas = zonas; }

    public async Task<IActionResult> Perfil(Guid id)
    {
        var profesional = await _obtener.EjecutarAsync(id);
        if (profesional == null) return NotFound();
        var calificaciones = await _calificaciones.EjecutarAsync(id);
        var enlace = await _enlaceWhatsApp.EjecutarAsync(id);
        ViewBag.Calificaciones = calificaciones;
        ViewBag.EnlaceWhatsApp = enlace.Url;
        return View(profesional);
    }

    [Authorize(Roles = "Profesional,Administrador")]
    public async Task<IActionResult> Registrar()
    {
        ViewBag.Rubros = await _rubros.ObtenerTodosAsync();
        ViewBag.Zonas = await _zonas.ObtenerTodosAsync();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Profesional,Administrador")]
    public async Task<IActionResult> Registrar(CrearProfesionalDto dto)
    {
        try
        {
            var resultado = await _registrar.EjecutarAsync(dto);
            TempData["Mensaje"] = "Profesional registrado exitosamente!";
            return RedirectToAction("Perfil", new { id = resultado.Id });
        }
        catch (InvalidOperationException ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.Rubros = await _rubros.ObtenerTodosAsync();
            ViewBag.Zonas = await _zonas.ObtenerTodosAsync();
            return View(dto);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Calificar(CrearCalificacionDto dto)
    {
        try { await _registrarCalificacion.EjecutarAsync(dto); TempData["Mensaje"] = "Calificacion registrada!"; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("Perfil", new { id = dto.ProfesionalId });
    }
}
