using TrabajoHuamanga.Aplicacion.DTOs;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Aplicacion.CasosDeUso.SugerenciaRubro;

public class SugerirRubro
{
    private readonly ISugeridorRubro _sugeridor;
    private readonly IRubroRepository _rubroRepo;

    public SugerirRubro(ISugeridorRubro sugeridor, IRubroRepository rubroRepo)
    {
        _sugeridor = sugeridor;
        _rubroRepo = rubroRepo;
    }

    public async Task<RubroDto?> EjecutarAsync(SugerenciaRubroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            throw new ArgumentException("La descripción es obligatoria.");

        // Llamar a OpenRouter/Gemini Flash para sugerir el rubro
        var nombreSugerido = await _sugeridor.SugerirRubroAsync(dto.Descripcion);

        // Buscar el rubro en la base de datos
        var rubro = await _rubroRepo.ObtenerPorNombreAsync(nombreSugerido);

        if (rubro != null)
            return new RubroDto(rubro.Id, rubro.Nombre, rubro.Descripcion, rubro.Icono);

        // Si no existe, devolver la sugerencia como texto
        return new RubroDto(Guid.Empty, nombreSugerido, "Sugerido por IA", null);
    }
}
