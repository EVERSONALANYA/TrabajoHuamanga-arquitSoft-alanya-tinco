# C4 - Nivel 4: Diagrama de código - Trabajo Huamanga

Muestra la estructura interna del componente **Reputación**, caso de uso `RegistrarCalificacion`.

```mermaid
classDiagram
    class ICalificacionRepository {
        <<interface>>
        +ExisteAsync(vecinoId, profesionalId) bool
        +GuardarAsync(calificacion) void
        +ObtenerPorProfesionalAsync(profesionalId) List~Calificacion~
    }

    class RegistrarCalificacion {
        -ICalificacionRepository repositorio
        -IEnumerable~IValidadorCalificacion~ validadores
        +EjecutarAsync(comando) Resultado
    }

    class IValidadorCalificacion {
        <<interface>>
        +ValidarAsync(comando) Task~Resultado~
    }

    class ValidadorPuntaje {
        +ValidarAsync(comando) Task~Resultado~
    }

    class ValidadorNoRepetida {
        +ValidarAsync(comando) Task~Resultado~
    }

    class Calificacion {
        +Guid Id
        +Guid VecinoId
        +Guid ProfesionalId
        +int Puntaje
        +string Comentario
        +DateTime Fecha
        +bool Oculta
        +Ocultar() void
    }

    class Profesional {
        +Guid Id
        +string Nombre
        +string Rubro
        +string Distrito
        +CalcularReputacion(calificaciones) decimal
    }

    class CalificacionRepository {
        +ExisteAsync(vecinoId, profesionalId) bool
        +GuardarAsync(calificacion) void
        +ObtenerPorProfesionalAsync(profesionalId) List~Calificacion~
    }

    RegistrarCalificacion --> ICalificacionRepository
    RegistrarCalificacion --> IValidadorCalificacion
    RegistrarCalificacion ..> Calificacion : crea
    ValidadorPuntaje ..|> IValidadorCalificacion
    ValidadorNoRepetida ..|> IValidadorCalificacion
    CalificacionRepository ..|> ICalificacionRepository
    Profesional ..> Calificacion : promedia
```

| Clase | Capa | Responsabilidad |
| --- | --- | --- |
| `RegistrarCalificacion` | Aplicación | Orquesta el caso de uso. |
| `IValidadorCalificacion` y validadores | Aplicación | Reglas de validación en cadena. |
| `Calificacion`, `Profesional` | Dominio | Entidades y regla del promedio de reputación. |
| `ICalificacionRepository` | Aplicación | Contrato de acceso a datos. |
| `CalificacionRepository` | Infraestructura | Implementación con EF Core y SQL Server (el boilerplate usa una versión en memoria). |
