using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SugerenciaController : ControllerBase
{
    private readonly SugerirRubro _sugerir;

    public SugerenciaController(SugerirRubro sugerir) => _sugerir = sugerir;

    /// <summary>POST api/sugerencia — IA sugiere rubro según descripción del problema</summary>
    [HttpPost]
    public async Task<ActionResult<SugerenciaRubroDto>> Sugerir([FromBody] SugerenciaRubroDto dto)
    {
        try
        {
            var resultado = await _sugerir.EjecutarAsync(dto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al consultar IA: " + ex.Message });
        }
    }
}
