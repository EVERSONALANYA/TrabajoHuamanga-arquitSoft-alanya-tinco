using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Aplicacion.CasosDeUso;
using TrabajoHuamanga.Dominio.Entidades;

namespace TrabajoHuamanga.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly BuscarProfesionales _buscar;
        private readonly ObtenerPerfil _obtenerPerfil;
        private readonly RegistrarProfesional _registrar;
        private readonly CalificarProfesional _calificar;
        private readonly ObtenerRubros _obtenerRubros;

        public ProfesionalesController(
            BuscarProfesionales buscar,
            ObtenerPerfil obtenerPerfil,
            RegistrarProfesional registrar,
            CalificarProfesional calificar,
            ObtenerRubros obtenerRubros)
        {
            _buscar = buscar;
            _obtenerPerfil = obtenerPerfil;
            _registrar = registrar;
            _calificar = calificar;
            _obtenerRubros = obtenerRubros;
        }

        /// <summary>
        /// Buscar profesionales por termino (nombre, rubro, distrito)
        /// </summary>
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string termino = "")
        {
            var resultados = await _buscar.EjecutarAsync(termino);
            return Ok(new { total = resultados.Count(), profesionales = resultados });
        }

        /// <summary>
        /// Obtener perfil completo de un profesional por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPerfil(int id)
        {
            var profesional = await _obtenerPerfil.EjecutarAsync(id);
            if (profesional == null) return NotFound(new { mensaje = "Profesional no encontrado" });
            return Ok(profesional);
        }

        /// <summary>
        /// Obtener lista de rubros disponibles
        /// </summary>
        [HttpGet("rubros")]
        public async Task<IActionResult> ObtenerRubros()
        {
            var rubros = await _obtenerRubros.EjecutarAsync();
            return Ok(rubros);
        }

        /// <summary>
        /// Registrar un nuevo profesional (requiere rol Profesional o Administrador)
        /// </summary>
        [HttpPost("registrar")]
        [Authorize(Roles = "Profesional,Administrador")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarProfesionalRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var profesional = new Profesional
            {
                Nombre = request.Nombre,
                Rubro = request.Rubro,
                Telefono = request.Telefono,
                Distrito = request.Distrito,
                Descripcion = request.Descripcion,
                FechaRegistro = DateTime.UtcNow
            };

            await _registrar.EjecutarAsync(profesional);
            return CreatedAtAction(nameof(ObtenerPerfil), new { id = profesional.Id }, profesional);
        }

        /// <summary>
        /// Calificar a un profesional (requiere autenticacion)
        /// </summary>
        [HttpPost("{id}/calificar")]
        [Authorize]
        public async Task<IActionResult> Calificar(int id, [FromBody] CalificarRequest request)
        {
            if (request.Puntuacion < 1 || request.Puntuacion > 5)
                return BadRequest(new { mensaje = "La puntuacion debe ser entre 1 y 5" });

            var calificacion = new Calificacion
            {
                ProfesionalId = id,
                Puntuacion = request.Puntuacion,
                Comentario = request.Comentario,
                Fecha = DateTime.UtcNow
            };

            await _calificar.EjecutarAsync(calificacion);
            return Ok(new { mensaje = "Calificacion registrada exitosamente" });
        }
    }

    public class RegistrarProfesionalRequest
    {
        public string Nombre { get; set; } = "";
        public string Rubro { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Distrito { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }

    public class CalificarRequest
    {
        public int Puntuacion { get; set; }
        public string Comentario { get; set; } = "";
    }
}
