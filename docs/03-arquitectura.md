# 3. Arquitectura de Alto y Bajo Nivel

## 3.1 Estilo Arquitectónico

**Monolito modular con Clean Architecture** — 4 capas con regla de dependencia hacia adentro.

## 3.2 Arquitectura de Alto Nivel (C4 — Nivel 1: Contexto)

```
                        ┌──────────────┐
                        │   Usuario    │
                        │  (Persona)   │
                        └──────┬───────┘
                               │ Busca profesionales,
                               │ califica, contacta
                               ▼
                    ┌─────────────────────┐
                    │  Trabajo Huamanga   │
                    │  (Sistema Web)      │
                    │                     │
                    │ ASP.NET Core 8      │
                    │ Clean Architecture  │
                    └───┬──────────┬──────┘
                        │          │
              ┌─────────▼──┐  ┌───▼──────────┐
              │ OpenRouter  │  │  WhatsApp    │
              │ (Gemini AI) │  │  (wa.me)     │
              │ Sugerencia  │  │  Contacto    │
              └─────────────┘  └──────────────┘
```

## 3.3 Arquitectura de Alto Nivel (C4 — Nivel 2: Contenedores)

```
┌──────────────────────────────────────────────────────────┐
│                    NGINX (Reverse Proxy)                  │
│                      Puerto 80                           │
└──────────┬───────────────────────────────┬───────────────┘
           │                               │
           ▼                               ▼
┌─────────────────────┐      ┌──────────────────────┐
│   Web MVC (.NET 8)  │      │   API REST (.NET 8)  │
│   Puerto 5000       │      │   Puerto 5001        │
│                     │      │                      │
│ • HomeController    │      │ • /api/profesionales │
│ • ProfesionalCtrl   │      │ • /api/busqueda      │
│ • AdminController   │      │ • /api/reputacion    │
│ • Razor Views       │      │ • /api/contacto      │
│ • Bootstrap 5       │      │ • /api/sugerencia    │
│                     │      │ • /api/moderacion    │
│                     │      │ • Swagger UI         │
└────────┬────────────┘      └──────────┬───────────┘
         │                              │
         │  Comparten las mismas        │
         │  capas internas:             │
         ▼                              ▼
┌──────────────────────────────────────────────────────────┐
│              Aplicación (Casos de Uso)                    │
│  RegistrarProfesional, BuscarProfesional,                │
│  RegistrarCalificacion, GenerarEnlaceWhatsApp,           │
│  SugerirRubro, ModerarCalificacion                       │
├──────────────────────────────────────────────────────────┤
│              Dominio (Entidades + Interfaces)             │
│  Profesional, Calificacion, Rubro, Zona                  │
│  IProfesionalRepository, ICacheBusqueda,                 │
│  ISugeridorRubro, IGeneradorEnlaceContacto               │
├──────────────────────────────────────────────────────────┤
│              Infraestructura (Implementaciones)           │
│  EF Core Repos, CacheBusquedaRedis,                     │
│  SugeridorRubroOpenRouter, GeneradorEnlaceWhatsApp       │
└─────────┬──────────────────────────────┬─────────────────┘
          │                              │
          ▼                              ▼
┌──────────────────┐          ┌──────────────────┐
│   SQL Server     │          │     Redis        │
│   Puerto 1433    │          │   Puerto 6379    │
│                  │          │                  │
│ • Profesionales  │          │ • Caché búsqueda │
│ • Calificaciones │          │ • TTL: 5 min     │
│ • Rubros         │          │ • ~2ms respuesta │
│ • Zonas          │          │                  │
└──────────────────┘          └──────────────────┘
```

## 3.4 Arquitectura de Bajo Nivel (C4 — Nivel 3: Componentes)

```
┌─────────────────────────────────────────────────────────────────────┐
│                    Capa de Presentación                              │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐                │
│  │HomeController │ │ProfesionalC. │ │AdminController│   Razor Views │
│  │(Búsqueda+IA) │ │(Perfil+Calif)│ │(Moderación)  │   Bootstrap 5 │
│  └──────┬───────┘ └──────┬───────┘ └──────┬───────┘                │
├─────────┼────────────────┼────────────────┼─────────────────────────┤
│         ▼                ▼                ▼                         │
│                    Capa de Aplicación                               │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐      │
│  │ Registrar  │ │  Buscar    │ │ Registrar  │ │  Generar   │      │
│  │Profesional │ │Profesional │ │Calificacion│ │EnlaceWApp  │      │
│  └──────┬─────┘ └─────┬──┬──┘ └──────┬─────┘ └──────┬─────┘      │
│         │              │  │           │              │              │
│  ┌──────┴─────┐  ┌─────┘  └──┐  ┌────┴──────┐ ┌────┴──────┐      │
│  │ SugerirRu. │  │           │  │ Moderar   │ │ Reportar  │      │
│  │   (IA)     │  │           │  │Calificac. │ │Calificac. │      │
│  └──────┬─────┘  │           │  └─────┬─────┘ └─────┬─────┘      │
├─────────┼────────┼───────────┼────────┼─────────────┼──────────────┤
│         ▼        ▼           ▼        ▼             ▼              │
│                    Capa de Dominio                                  │
│  ┌────────────────┐  ┌──────────────┐  ┌─────────────────────────┐│
│  │   Entidades    │  │  Interfaces  │  │   Reglas de Negocio     ││
│  │ • Profesional  │  │ • IProf.Repo │  │ • Puntaje 1-5           ││
│  │ • Calificacion │  │ • ICalif.Rep │  │ • No duplicar calif.    ││
│  │ • Rubro        │  │ • ICacheBusq │  │ • Factory methods       ││
│  │ • Zona         │  │ • ISugeridor │  │ • Validaciones en ent.  ││
│  │                │  │ • IGenerador │  │                         ││
│  └────────────────┘  └──────┬───────┘  └─────────────────────────┘│
├─────────────────────────────┼──────────────────────────────────────┤
│         ▲                   ▼  (implementa)                        │
│                    Capa de Infraestructura                          │
│  ┌────────────────┐  ┌──────────────┐  ┌─────────────────────────┐│
│  │  Repositorios  │  │  Adaptadores │  │   Caché                 ││
│  │ • ProfesionalR │  │ • WhatsApp   │  │ • CacheBusquedaRedis    ││
│  │ • CalificacionR│  │ • OpenRouter │  │   (IDistributedCache)   ││
│  │ • RubroRepo    │  │              │  │   TTL: 5 minutos        ││
│  │ • ZonaRepo     │  │              │  │   Degradación elegante  ││
│  │ (EF Core)      │  │              │  │                         ││
│  └────────────────┘  └──────────────┘  └─────────────────────────┘│
└─────────────────────────────────────────────────────────────────────┘
```

## 3.5 Reglas de Dependencia (Clean Architecture)

1. **Dominio** no depende de ninguna otra capa (cero referencias externas)
2. **Aplicación** depende solo de Dominio (usa interfaces, no implementaciones)
3. **Infraestructura** implementa las interfaces definidas en Dominio
4. **Presentación** orquesta Aplicación e Infraestructura vía inyección de dependencias
5. **Inversión de dependencias**: la capa de Infraestructura depende de las abstracciones del Dominio, no al revés
6. Los DTOs de Aplicación son la frontera entre capas; las entidades del Dominio nunca se exponen directamente

## 3.6 Decisiones Arquitectónicas (ADR)

| Decisión | Opción elegida | Justificación |
|----------|---------------|---------------|
| Estilo | Clean Architecture | Separación clara, testeable, mantenible |
| Backend | ASP.NET Core 8 | Performance, ecosistema maduro, Docker nativo |
| ORM | EF Core 8 | Productividad, migraciones, LINQ |
| Caché | Redis | Sub-milisegundo, distribuido, estándar industria |
| BD | SQL Server | Robusto, integración nativa con .NET |
| IA | OpenRouter→Gemini | Costo mínimo (~$0.02/mes), API estándar |
| Contacto | WhatsApp wa.me | Gratis, sin API token, alta adopción local |
| Frontend | Razor + Bootstrap | Productivo, responsive, SEO-friendly |
| Deploy | Docker Compose | Reproducible, escalable, portable |
