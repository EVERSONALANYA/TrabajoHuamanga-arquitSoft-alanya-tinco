using System.Net.Http.Json;
using System.Text.Json;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Infraestructura.Adaptadores;

/// <summary>
/// Adaptador de OpenRouter → Gemini Flash para sugerencia de rubro.
/// Costo: ~$0.02/mes.
/// </summary>
public class SugeridorRubroOpenRouter : ISugeridorRubro
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;

    public SugeridorRubroOpenRouter(HttpClient http, string apiKey, string model = "google/gemini-flash-1.5")
    {
        _http = http;
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> SugerirRubroAsync(string descripcionProblema)
    {
        var request = new
        {
            model = _model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Eres un clasificador de rubros de servicios para Huamanga, Perú. " +
                              "El usuario describe un problema o necesidad. Responde SOLO con el nombre del rubro " +
                              "más apropiado de esta lista: Electricista, Gasfitero, Albañil, Pintor, Carpintero, " +
                              "Cerrajero, Tutor académico, Técnico en computadoras, Soldador, Jardinero. " +
                              "Si no encaja en ninguno, responde con el rubro más cercano."
                },
                new
                {
                    role = "user",
                    content = descripcionProblema
                }
            },
            max_tokens = 50,
            temperature = 0.1
        };

        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _http.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var rubro = json.GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "Electricista";

        return rubro.Trim();
    }
}
