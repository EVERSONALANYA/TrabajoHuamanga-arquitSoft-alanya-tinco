# Principios de diseño (SOLID) - Trabajo Huamanga

| Principio | Cómo se aplica en el proyecto |
| --- | --- |
| SRP - Responsabilidad única | Cada clase tiene una razón para cambiar: `RegistrarCalificacion` solo orquesta el caso de uso; `CalificacionRepository` solo guarda; `Calificacion` solo contiene sus reglas. |
| OCP - Abierto/cerrado | Para agregar otro proveedor de IA se crea una nueva clase que implementa `ISugeridorRubro`; no se modifica el caso de uso. |
| LSP - Sustitución de Liskov | `SugeridorRubroOpenRouter` y `SugeridorRubroManual` cumplen el mismo contrato y pueden reemplazarse sin romper `SugerirRubro`. |
| ISP - Segregación de interfaces | Interfaces pequeñas y específicas (`ISugeridorRubro`, `IGeneradorEnlaceContacto`, `ICacheBusqueda`) en lugar de una interfaz grande de servicios externos. |
| DIP - Inversión de dependencias | Los casos de uso dependen de interfaces definidas en Aplicación; las implementaciones se inyectan con la inyección de dependencias de ASP.NET Core. |

## Ejemplos

### SRP

Problema: una sola clase valida, calcula, guarda y avisa.
Solución: separar en caso de uso, validadores y repositorio.

### OCP y LSP

```csharp
public interface ISugeridorRubro
{
    Task<string> SugerirAsync(string descripcionProblema);
}

public class SugeridorRubroOpenRouter : ISugeridorRubro { /* usa la IA */ }
public class SugeridorRubroManual : ISugeridorRubro { /* devuelve la lista de rubros */ }
```

### DIP

```csharp
public class SugerirRubro
{
    private readonly ISugeridorRubro _sugeridor;

    public SugerirRubro(ISugeridorRubro sugeridor)
    {
        _sugeridor = sugeridor;
    }

    public Task<string> EjecutarAsync(string descripcion) => _sugeridor.SugerirAsync(descripcion);
}
```

En `Program.cs` se registra la implementación concreta:

```csharp
builder.Services.AddScoped<ISugeridorRubro, SugeridorRubroOpenRouter>();
```

Cambiar de proveedor implica cambiar esa línea, no el caso de uso.

## Relación con TDD

Como los casos de uso dependen de interfaces, en las pruebas unitarias se reemplazan los repositorios y servicios externos por dobles de prueba, sin necesidad de base de datos ni de internet.
