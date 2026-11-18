using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReputacionController : ControllerBase
{
    private readonly RegistrarCalificacion _registrar;
    private readonly ObtenerCalificaciones _obtener;
    private readonly ReportarCalificacionCasoUso _reportar;

    public ReputacionController(
        RegistrarCalificacion registrar,
        ObtenerCalificaciones obtener,
        ReportarCalificacionCasoUso reportar)
    {
        _registrar = registrar;
        _obtener = obtener;
        _reportar = reportar;
    }

    /// <summary>GET api/reputacion/{profesionalId} — Obtener calificaciones de un profesional</summary>
    [HttpGet("{profesionalId:guid}")]
    public async Task<ActionResult<List<CalificacionDto>>> ObtenerCalificaciones(Guid profesionalId)
    {
        var resultado = await _obtener.EjecutarAsync(profesionalId);
        return Ok(resultado);
    }

    /// <summary>POST api/reputacion — Registrar nueva calificación</summary>
    [HttpPost]
    public async Task<IActionResult> Calificar([FromBody] CrearCalificacionDto dto)
    {
        try
        {
            await _registrar.EjecutarAsync(dto);
            return Ok(new { mensaje = "Calificación registrada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>POST api/reputacion/reportar — Reportar una calificación inapropiada</summary>
    [HttpPost("reportar")]
    public async Task<IActionResult> Reportar([FromBody] ReportarCalificacionDto dto)
    {
        try
        {
            await _reportar.EjecutarAsync(dto);
            return Ok(new { mensaje = "Calificación reportada para revisión" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
