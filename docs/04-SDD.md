# 4. Software Design Document (SDD)

## 4.1 Introducción

Este documento describe el diseño detallado del software "Trabajo Huamanga", siguiendo el estándar IEEE 1016.

### Propósito
Describir la arquitectura, componentes, interfaces y datos del sistema para guiar la implementación.

### Alcance
Directorio web de profesionales de servicios en Huamanga con 6 módulos funcionales.

## 4.2 Diseño Arquitectónico

### 4.2.1 Patrón: Clean Architecture (Monolito Modular)

La solución se organiza en 6 proyectos .NET:

| Proyecto | Capa | Responsabilidad |
|----------|------|----------------|
| TrabajoHuamanga.Dominio | Dominio | Entidades, interfaces, reglas de negocio |
| TrabajoHuamanga.Aplicacion | Aplicación | Casos de uso, DTOs, orquestación |
| TrabajoHuamanga.Infraestructura | Infraestructura | EF Core, Redis, OpenRouter, WhatsApp |
| TrabajoHuamanga.API | Presentación | REST API + Swagger |
| TrabajoHuamanga.Web | Presentación | MVC + Razor Views |
| TrabajoHuamanga.Tests | Testing | xUnit + Moq |

### 4.2.2 Flujo de dependencias

```
Presentación ──▶ Aplicación ──▶ Dominio ◀── Infraestructura
     (usa)          (usa)      (define)    (implementa)
```

## 4.3 Diseño Detallado de Módulos

### 4.3.1 Módulo Profesionales
- **Entidad**: `Profesional` — Factory method `Crear()`, validación en constructor
- **Casos de uso**: `RegistrarProfesional`, `ObtenerProfesional`
- **Repositorio**: `IProfesionalRepository` → `ProfesionalRepository` (EF Core)
- **Endpoints**: `POST /api/profesionales`, `GET /api/profesionales/{id}`

### 4.3.2 Módulo Búsqueda
- **Caso de uso**: `BuscarProfesional`
- **Estrategia Cache-Aside**: consulta Redis primero (clave: `busqueda:{rubro}:{zona}:{termino}`), si miss → SQL Server → guardar en Redis con TTL 5 min
- **Rendimiento**: ~2ms con caché vs ~200ms sin caché
- **Endpoints**: `GET /api/busqueda?rubroId=&zonaId=&termino=`

### 4.3.3 Módulo Reputación
- **Entidad**: `Calificacion` — Puntaje 1-5, reportable, ocultable
- **Casos de uso**: `RegistrarCalificacion`, `ObtenerCalificaciones`
- **Regla**: Un usuario no puede calificar dos veces al mismo profesional
- **Al calificar**: se invalida el caché de búsqueda del profesional

### 4.3.4 Módulo Contacto
- **Adaptador**: `GeneradorEnlaceWhatsApp`
- **Formato**: `https://wa.me/51{telefono}?text={mensaje_codificado}`
- **Costo**: $0 (usa enlaces wa.me, no API de WhatsApp)

### 4.3.5 Módulo Sugerencia de Rubro (IA)
- **Adaptador**: `SugeridorRubroOpenRouter`
- **Modelo**: Google Gemini Flash 1.5 vía OpenRouter
- **Prompt**: Clasificador que mapea descripción → uno de 10 rubros
- **Costo**: ~$0.02/mes (50 tokens por consulta)

### 4.3.6 Módulo Moderación
- **Casos de uso**: `ModerarCalificacion`, `ReportarCalificacionCasoUso`
- **Flujo**: Usuario reporta → Admin ve reportadas → Ocultar/Restaurar

## 4.4 Diseño de Interfaces

### 4.4.1 API REST

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/profesionales | Listar profesionales activos |
| GET | /api/profesionales/{id} | Obtener profesional por ID |
| POST | /api/profesionales | Registrar profesional |
| GET | /api/busqueda | Buscar por rubro/zona/término |
| GET | /api/reputacion/{profId} | Calificaciones de un profesional |
| POST | /api/reputacion | Registrar calificación |
| POST | /api/reputacion/reportar | Reportar calificación |
| GET | /api/contacto/{profId} | Generar enlace WhatsApp |
| POST | /api/sugerencia | Sugerir rubro por IA |
| GET | /api/moderacion/reportadas | Listar reportadas |
| POST | /api/moderacion/ocultar/{id} | Ocultar calificación |
| POST | /api/moderacion/restaurar/{id} | Restaurar calificación |
| GET | /api/catalogos/rubros | Listar rubros |
| GET | /api/catalogos/zonas | Listar zonas |

### 4.4.2 Vistas MVC

| Vista | Ruta | Actor | Descripción |
|-------|------|-------|-------------|
| Home/Index | / | Usuario | Buscador + resultados + IA |
| Profesional/Perfil | /Profesional/Perfil/{id} | Usuario | Perfil + calificaciones + WhatsApp |
| Profesional/Registrar | /Profesional/Registrar | Profesional | Formulario de registro |
| Admin/Index | /Admin | Admin | Panel de moderación |

## 4.5 Diseño de Datos

### DTOs (Data Transfer Objects)
```csharp
record ProfesionalDto(string Id, string NombreCompleto, string Telefono,
    string Descripcion, string? FotoUrl, string RubroNombre,
    string ZonaNombre, double PromedioCalificacion, int TotalCalificaciones, bool Activo);

record CrearProfesionalDto(string NombreCompleto, string Telefono,
    string Descripcion, string? FotoUrl, Guid RubroId, Guid ZonaId);

record BusquedaDto(Guid? RubroId, Guid? ZonaId, string? Termino);

record CalificacionDto(string Id, string ProfesionalId, string NombreUsuario,
    int Puntaje, string Comentario, bool Reportada,
    string? MotivoReporte, DateTime FechaCreacion);

record SugerenciaRubroDto(string Descripcion, string? RubroSugerido, Guid? RubroId);
```

## 4.6 Estrategia de Caché

```
Petición de búsqueda
        │
        ▼
  ┌─────────────┐     HIT      ┌──────────┐
  │ Redis Cache  │─────────────▶│ Respuesta│ (~2ms)
  │ (5 min TTL)  │              │  rápida  │
  └──────┬───────┘              └──────────┘
         │ MISS
         ▼
  ┌─────────────┐              ┌──────────┐
  │ SQL Server   │─────────────▶│ Guardar  │
  │ (consulta)   │              │ en Redis │
  └──────────────┘              └──────────┘
                                     │
                                     ▼ (~200ms primera vez)
```

- **Invalidación**: Al registrar una calificación, se elimina el caché del profesional afectado
- **Degradación elegante**: Si Redis falla, la app funciona directamente con SQL Server
