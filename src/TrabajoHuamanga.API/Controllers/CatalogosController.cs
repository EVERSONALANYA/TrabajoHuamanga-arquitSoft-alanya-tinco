using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Dominio.Interfaces;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly IRubroRepository _rubros;
    private readonly IZonaRepository _zonas;

    public CatalogosController(IRubroRepository rubros, IZonaRepository zonas)
    {
        _rubros = rubros;
        _zonas = zonas;
    }

    /// <summary>GET api/catalogos/rubros — Listar todos los rubros</summary>
    [HttpGet("rubros")]
    public async Task<ActionResult<List<RubroDto>>> ObtenerRubros()
    {
        var rubros = await _rubros.ObtenerTodosAsync();
        return Ok(rubros.Select(r => new RubroDto(r.Id, r.Nombre, r.Descripcion, r.Icono)).ToList());
    }

    /// <summary>GET api/catalogos/zonas — Listar todas las zonas</summary>
    [HttpGet("zonas")]
    public async Task<ActionResult<List<ZonaDto>>> ObtenerZonas()
    {
        var zonas = await _zonas.ObtenerTodosAsync();
        return Ok(zonas.Select(z => new ZonaDto(z.Id, z.Nombre, z.Distrito)).ToList());
    }
}
