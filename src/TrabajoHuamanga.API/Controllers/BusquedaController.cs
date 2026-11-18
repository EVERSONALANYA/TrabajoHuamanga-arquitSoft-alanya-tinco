using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusquedaController : ControllerBase
{
    private readonly BuscarProfesional _buscar;

    public BusquedaController(BuscarProfesional buscar) => _buscar = buscar;

    /// <summary>GET api/busqueda?rubroId=...&zonaId=...&termino=... — Buscar profesionales</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProfesionalDto>>> Buscar([FromQuery] BusquedaDto dto)
    {
        var resultados = await _buscar.EjecutarAsync(dto);
        return Ok(resultados);
    }
}
