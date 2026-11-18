using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.Contacto;

public class GenerarEnlaceWhatsApp
{
    private readonly IProfesionalRepository _repo;
    private readonly IGeneradorEnlaceContacto _generador;

    public GenerarEnlaceWhatsApp(IProfesionalRepository repo, IGeneradorEnlaceContacto generador)
    {
        _repo = repo;
        _generador = generador;
    }

    public async Task<EnlaceContactoDto> EjecutarAsync(Guid profesionalId)
    {
        var profesional = await _repo.ObtenerPorIdAsync(profesionalId)
            ?? throw new ArgumentException("El profesional no existe.");

        var enlace = _generador.GenerarEnlace(
            profesional.Telefono,
            profesional.NombreCompleto,
            profesional.Rubro?.Nombre ?? "servicio");

        return new EnlaceContactoDto(enlace, profesional.NombreCompleto);
    }
}
