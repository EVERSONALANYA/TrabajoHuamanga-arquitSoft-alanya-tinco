using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Contacto;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactoController : ControllerBase
{
    private readonly GenerarEnlaceWhatsApp _generarEnlace;

    public ContactoController(GenerarEnlaceWhatsApp generarEnlace) => _generarEnlace = generarEnlace;

    /// <summary>GET api/contacto/{profesionalId} — Generar enlace WhatsApp para contactar</summary>
    [HttpGet("{profesionalId:guid}")]
    public async Task<ActionResult<EnlaceContactoDto>> GenerarEnlace(Guid profesionalId)
    {
        try
        {
            var resultado = await _generarEnlace.EjecutarAsync(profesionalId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
