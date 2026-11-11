using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Moderacion;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModeracionController : ControllerBase
{
    private readonly ModerarCalificacion _moderar;

    public ModeracionController(ModerarCalificacion moderar) => _moderar = moderar;

    /// <summary>GET api/moderacion/reportadas — Listar calificaciones reportadas</summary>
    [HttpGet("reportadas")]
    public async Task<ActionResult<List<CalificacionDto>>> ObtenerReportadas()
    {
        var resultado = await _moderar.ObtenerReportadasAsync();
        return Ok(resultado);
    }

    /// <summary>POST api/moderacion/ocultar/{id} — Ocultar calificación inapropiada</summary>
    [HttpPost("ocultar/{id:guid}")]
    public async Task<IActionResult> Ocultar(Guid id)
    {
        try
        {
            await _moderar.OcultarAsync(id);
            return Ok(new { mensaje = "Calificación ocultada" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>POST api/moderacion/restaurar/{id} — Restaurar calificación ocultada</summary>
    [HttpPost("restaurar/{id:guid}")]
    public async Task<IActionResult> Restaurar(Guid id)
    {
        try
        {
            await _moderar.RestaurarAsync(id);
            return Ok(new { mensaje = "Calificación restaurada" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
