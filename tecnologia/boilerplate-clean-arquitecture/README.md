# Boilerplate Clean Architecture - Trabajo Huamanga

Ejemplo mínimo del enfoque Clean Architecture: el caso de uso `RegistrarCalificacion` (módulo Reputación) con dominio, aplicación, infraestructura y pruebas. Corresponde al diagrama C4 nivel 4 (`docs/04-modelo-c4/Nivel4-DiagramadeCodigo.md`).

## Estructura

```text
src/
  TrabajoHuamanga.Dominio/            Calificacion, Profesional (reglas de negocio)
  TrabajoHuamanga.Aplicacion/         caso de uso, puertos y validadores
  TrabajoHuamanga.Infraestructura/    implementacion del repositorio (en memoria)
tests/
  TrabajoHuamanga.Pruebas/            pruebas unitarias (xUnit)
```

## Regla de dependencia

`Dominio` no depende de nadie. `Aplicacion` depende de `Dominio`. `Infraestructura` depende de `Aplicacion`. Los casos de uso conocen solo interfaces (`ICalificacionRepository`, `IValidadorCalificacion`).

## Cómo ejecutar las pruebas

Requiere el SDK de .NET 8.

```bash
dotnet test tests/TrabajoHuamanga.Pruebas
```

## Qué demuestran las pruebas

- Una calificación válida se guarda.
- Un puntaje fuera del rango 1 a 5 se rechaza.
- Una calificación repetida del mismo vecino al mismo profesional se rechaza.
- La reputación ignora las calificaciones ocultas por moderación.
