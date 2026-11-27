using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Busqueda;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Profesionales;
using TrabajoHuamanga.Aplicacion.CasosDeUso.Reputacion;
using TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;
using TrabajoHuamanga.Aplicacion.DTOs;

namespace TrabajoHuamanga.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly BuscarProfesional _buscar;
        private readonly ObtenerProfesional _obtenerProfesional;
        private readonly RegistrarProfesional _registrar;
        private readonly RegistrarCalificacion _registrarCalificacion;
        private readonly ObtenerCalificaciones _obtenerCalificaciones;
        private readonly SugerirRubro _sugerirRubro;

        public ProfesionalesController(
            BuscarProfesional buscar,
            ObtenerProfesional obtenerProfesional,
            RegistrarProfesional registrar,
            RegistrarCalificacion registrarCalificacion,
            ObtenerCalificaciones obtenerCalificaciones,
            SugerirRubro sugerirRubro)
        {
            _buscar = buscar;
            _obtenerProfesional = obtenerProfesional;
            _registrar = registrar;
            _registrarCalificacion = registrarCalificacion;
            _obtenerCalificaciones = obtenerCalificaciones;
            _sugerirRubro = sugerirRubro;
        }

        /// <summary>
        /// Buscar profesionales por rubro, zona y/o termino
        /// </summary>
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] Guid? rubroId, [FromQuery] Guid? zonaId, [FromQuery] string? termino)
        {
            var dto = new BusquedaDto(rubroId, zonaId, termino);
            var resultados = await _buscar.EjecutarAsync(dto);
            return Ok(new { total = resultados.Count, profesionales = resultados });
        }

        /// <summary>
        /// Obtener perfil de un profesional por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPerfil(Guid id)
        {
            var profesional = await _obtenerProfesional.EjecutarAsync(id);
            if (profesional == null) return NotFound(new { mensaje = "Profesional no encontrado" });
            return Ok(profesional);
        }

        /// <summary>
        /// Obtener calificaciones de un profesional
        /// </summary>
        [HttpGet("{id}/calificaciones")]
        public async Task<IActionResult> ObtenerCalificaciones(Guid id)
        {
            var calificaciones = await _obtenerCalificaciones.EjecutarAsync(id);
            return Ok(calificaciones);
        }

        /// <summary>
        /// Registrar un nuevo profesional (requiere rol Profesional o Administrador)
        /// </summary>
        [HttpPost("registrar")]
        [Authorize(Roles = "Profesional,Administrador")]
        public async Task<IActionResult> Registrar([FromBody] CrearProfesionalDto dto)
        {
            try
            {
                var resultado = await _registrar.EjecutarAsync(dto);
                return CreatedAtAction(nameof(ObtenerPerfil), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Calificar a un profesional (requiere autenticacion)
        /// </summary>
        [HttpPost("{id}/calificar")]
        [Authorize]
        public async Task<IActionResult> Calificar(Guid id, [FromBody] CalificarRequest request)
        {
            try
            {
                var dto = new CrearCalificacionDto(id, request.NombreUsuario, request.Puntaje, request.Comentario);
                var resultado = await _registrarCalificacion.EjecutarAsync(dto);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Sugerir rubro usando IA (OpenRouter/Gemini)
        /// </summary>
        [HttpPost("sugerir-rubro")]
        public async Task<IActionResult> SugerirRubro([FromBody] SugerenciaRubroDto dto)
        {
            try
            {
                var resultado = await _sugerirRubro.EjecutarAsync(dto);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

    public class CalificarRequest
    {
        public string NombreUsuario { get; set; } = "";
        public int Puntaje { get; set; }
        public string? Comentario { get; set; }
    }
}
