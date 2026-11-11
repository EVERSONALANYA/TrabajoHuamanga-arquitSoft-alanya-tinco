using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfesionalesController : ControllerBase
{
    private readonly RegistrarProfesional _registrar;
    private readonly ObtenerProfesional _obtener;

    public ProfesionalesController(RegistrarProfesional registrar, ObtenerProfesional obtener)
    {
        _registrar = registrar;
        _obtener = obtener;
    }

    /// <summary>GET api/profesionales — Listar todos los profesionales activos</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProfesionalDto>>> ObtenerTodos()
    {
        var resultado = await _obtener.ObtenerTodosAsync();
        return Ok(resultado);
    }

    /// <summary>GET api/profesionales/{id} — Obtener un profesional por ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfesionalDto>> ObtenerPorId(Guid id)
    {
        var resultado = await _obtener.EjecutarAsync(id);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    /// <summary>POST api/profesionales — Registrar nuevo profesional</summary>
    [HttpPost]
    public async Task<ActionResult<ProfesionalDto>> Registrar([FromBody] CrearProfesionalDto dto)
    {
        try
        {
            var resultado = await _registrar.EjecutarAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
